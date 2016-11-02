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

using TPoint = NetTopologySuite.Geometries.Point;

namespace WeAuto
{
	/// <summary>
	/// Description of FindInnerRoomUtils.
	/// </summary>
	public class InnerRoom
	{
		public DimData DimH;
		public DimData DimV;
		
		public IPolygon m_innerRoom;
		public InnerRoom(IPolygon pg)
		{
			m_innerRoom = pg;
		}
		
		public void GenDim(bool isLeft, bool isBottom)
		{
			Envelope enp = m_innerRoom.EnvelopeInternal;
			
			DimH = new DimData();
			DimV = new DimData();
			List<double> us = new List<double>();
			List<double> vs = new List<double>();
			
			XYZ center = GetCenter();
			
			us.Add(enp.MinX);
			us.Add(center.X);
			us.Add(enp.MaxX);
			
			vs.Add(enp.MinY);
			vs.Add(center.Y);
			vs.Add(enp.MaxY);

			double off = ThisApplication.Settings.DimOffset;;
			
			{
				double v0 = vs[0];
				double mov = -off;
				if(!isBottom)
				{
					v0 = vs[2];
					mov = + off;	
				}
				XYZ hStart = new XYZ(us[0], v0 + mov, 0);
				XYZ hEnd =  new XYZ(us[us.Count - 1], v0 + mov, 0);
				Line hLn = Line.CreateBound(hStart, hEnd);	
				DimH.LocLn = hLn;
				
				for(int i = 0; i<us.Count; i++)
				{
					XYZ p0 = new XYZ(us[i], v0 - mov, 0);
					XYZ p1 = new XYZ(us[i], v0 + mov, 0);
					Line lnSeg = Line.CreateBound(p0, p1);
					DimH.RefLns.Add(lnSeg);
				}
			}
			
			{
				double u0 = us[0];
				double mov = -off;
				if(!isLeft)
				{
					u0 = us[2];
					mov = + off;					
				}
				XYZ vStart = new XYZ(u0 + mov, vs[0], 0);
				XYZ vEnd =  new XYZ(u0 + mov, vs[vs.Count - 1], 0);
				Line vLn = Line.CreateBound(vStart, vEnd);
				DimV.LocLn = vLn;
				
				for(int i = 0; i<vs.Count; i++)
				{
					XYZ p0 = new XYZ(u0 - mov, vs[i], 0);
					XYZ p1 = new XYZ(u0 + mov, vs[i], 0);
					Line lnSeg = Line.CreateBound(p0, p1);
					DimV.RefLns.Add(lnSeg);
				}
			}
		}
		
		public XYZ GetCenter()
		{
			IPoint pnt = m_innerRoom.Centroid;
            return new XYZ(pnt.X, pnt.Y, 0);
		}
		
		public double Area
		{
			get
			{
				return m_innerRoom.Area;
			}
		}
		
		public bool IsInside(XYZ pnt)
		{
			TPoint tpnt = new TPoint(ToCoord(pnt));
			return m_innerRoom.Contains(tpnt);
		}
		
		public static List<InnerRoom> FindInnerRooms(List<RVLine> polygon, List<RVLine> lines)
		{
			if(lines == null || lines.Count == 0)
			{
				return null;
			}
			
			PrecisionModel pm = new PrecisionModel();
			GeometryFactory fact = new GeometryFactory(pm);
			
			CoordinateList pnts = new CoordinateList();
			foreach (Line ln in polygon) 
			{
				pnts.Add(ToCoord(ln.GetEndPoint(0)), true);
			}
			pnts.Add(pnts[0], true);
			
			LinearRing lr = new LinearRing(pnts.ToArray());
			IPolygon plg = fact.CreatePolygon(lr);
			
			List<ILineString> lss = new List<ILineString>();
			foreach (Line ln in lines) 
			{
				Line exLn = Extend(ln, 2);
				ILineString ls = ToLS(exLn, fact);

				lss.Add(ls);
			}
			
			MultiLineString mls = new MultiLineString(lss.ToArray());
			
		 	var nodedLinework = plg.Boundary.Union(mls);
		 	var polygons = Polygonize(nodedLinework);
		 	
		 	List<InnerRoom> inRms = new List<InnerRoom>();
		 	if(polygons.NumGeometries <= 1)
		 	{
		 		return null;
		 	}
            for (var i = 0; i < polygons.NumGeometries; i++)
            {
                var candpoly = (IPolygon)polygons.GetGeometryN(i);
                if(candpoly.Area > 150)
                {
                	continue;
                }
                if (plg.Contains(candpoly.InteriorPoint))
                {
                    inRms.Add(new InnerRoom(candpoly));
                }
            }
            return inRms;
		}
		
		static Line Extend(Line ln, double len)
		{
			XYZ start = ln.GetEndPoint(0);
			start -= ln.Direction * len;
			XYZ end = ln.GetEndPoint(1);
			end += ln.Direction * len;
			return Line.CreateBound(start, end);
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
		
		public static ILineString ToLS(Line ln, GeometryFactory fact)
		{
			Coordinate[] cds = new Coordinate[2]{ToCoord(ln.GetEndPoint(0)), ToCoord(ln.GetEndPoint(1))};
			ILineString ls = fact.CreateLineString(cds);
			return ls;
		}
	}
}
