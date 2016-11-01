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
using Rhino.Geometry;
using RHArc = Autodesk.Revit.DB.Arc;
using RHCurve = Rhino.Geometry.Curve;
using RHLine = Rhino.Geometry.LineCurve;

using RVLine = Autodesk.Revit.DB.Line;

namespace WeAuto
{
	/// <summary>
	/// Description of FindInnerRoomUtils.
	/// </summary>
	public static class FindInnerRoomUtils
	{
		public static List<RVLine> FindInnerRooms(List<RHLine> polygon, List<RHLine> lines)
		{
			CoordinateList pnts = new CoordinateList();
			foreach (RHLine ln in polygon) 
			{
				pnts.Add(ToCoord(ln.PointAtStart), true);
			}
			pnts.Add(pnts[0], true);
			
			LinearRing lr = new LinearRing(pnts.ToArray());
			Polygon plg = new Polygon(lr);
			
			List<LineString> lss = new List<LineString>();
			foreach (RHLine ln in lines) 
			{
				lss.Add(ToLS(ln));
			}
			
			MultiLineString mls = new MultiLineString(lss.ToArray());
			
		 	var nodedLinework = plg.Boundary.Union(mls);
		 	var polygons = Polygonize(nodedLinework);
		 	
		 	List<RVLine> rslt = new List<RVLine>();
            for (var i = 0; i < polygons.NumGeometries; i++)
            {
                var candpoly = (IPolygon)polygons.GetGeometryN(i);
                if (plg.Contains(candpoly.InteriorPoint))
                {
                    
                }
            }
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
		
		public static Coordinate ToCoord(Point3d pn)
		{
			return new Coordinate(pn.X, pn.Y, 0);
		}
		
		public static LineString ToLS(RHLine ln)
		{
			Coordinate[] cds = new Coordinate[2]{ToCoord(ln.PointAtStart), ToCoord(ln.PointAtEnd)};
			return new LineString(cds);
		}
	}
}
