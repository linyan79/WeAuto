/*
 * Created by SharpDevelop.
 * User: lyan
 * Date: 10/27/2016
 * Time: 6:02 PM
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Collections.Generic;
using GeoAPI.Geometries;
using NetTopologySuite.Geometries;
using NetTopologySuite.Geometries.Utilities;
using NetTopologySuite.IO;
using NetTopologySuite.Operation.Polygonize;

using Autodesk.Revit.DB;

using RHArc = Autodesk.Revit.DB.Arc;
using RVLine = Autodesk.Revit.DB.Line;
using RVPnt = Autodesk.Revit.DB.XYZ;

namespace WeAuto
{
	/// <summary>
	/// Description of FindInnerRoomUtils.
	/// </summary>
	public static class FindInnerRoomUtils
	{
		public static List<RVPnt> FindInnerRooms(List<RVLine> polygon, List<RVLine> lines)
		{
			CoordinateList pnts = new CoordinateList();
			foreach (Line ln in polygon) 
			{
				pnts.Add(ToCoord(ln.GetEndPoint(0)), true);
			}
			pnts.Add(pnts[0], true);
			
			LinearRing lr = new LinearRing(pnts.ToArray());
			Polygon plg = new Polygon(lr);
			
			List<LineString> lss = new List<LineString>();
			foreach (Line ln in lines) 
			{
				lss.Add(ToLS(ln));
			}
			
			MultiLineString mls = new MultiLineString(lss.ToArray());
			
		 	var nodedLinework = plg.Boundary.Union(mls);
		 	var polygons = Polygonize(nodedLinework);
		 	
		 	List<RVPnt> rslt = new List<RVPnt>();
            for (var i = 0; i < polygons.NumGeometries; i++)
            {
                var candpoly = (IPolygon)polygons.GetGeometryN(i);
                if (plg.Contains(candpoly.InteriorPoint))
                {
                    IPoint pnt = candpoly.Centroid;
                    RVPnt rpnt = new Autodesk.Revit.DB.XYZ(DrawUtils.ToFt(pnt.X), DrawUtils.ToFt(pnt.Y), 0);
                    rslt.Add(rpnt);
                }
            }
            return rslt;
		}
		
        internal static IGeometry Polygonize(IGeometry geometry)
        {
            var lines = LineStringExtracter.GetLines(geometry);
            var polygonizer = new Polygonizer(false);
            polygonizer.Add(lines);
            var polys = new List<IGeometry>(polygonizer.GetPolygons());

            var polyArray = GeometryFactory.ToGeometryArray(polys);
            return geometry.Factory.BuildGeometry(polyArray);
        }
		
		public static Coordinate ToCoord(XYZ pn)
		{
			return new Coordinate(pn.X, pn.Y, 0);
		}
		
		public static LineString ToLS(Line ln)
		{
			Coordinate[] cds = new Coordinate[2]{ToCoord(ln.GetEndPoint(0)), ToCoord(ln.GetEndPoint(1))};
			return new LineString(cds);
		}
	}
}
