/*
 * Created by SharpDevelop.
 * User: lyan
 * Date: 10/20/2016
 * Time: 3:03 PM
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

using Autodesk.Revit.DB;
using Rhino.Geometry;
using Rhino;
using Rhino.Geometry;
using RApp = Autodesk.Revit.ApplicationServices.Application;
using RDoc = Autodesk.Revit.DB.Document;
using RHArc = Autodesk.Revit.DB.Arc;
using RHCurve = Rhino.Geometry.Curve;
using RHLine = Rhino.Geometry.LineCurve;
using RMBD = Autodesk.Revit.DB.BoundarySegment;
using RVArc = Autodesk.Revit.DB.Arc;
using RVCurve = Autodesk.Revit.DB.Curve;
using RView = Autodesk.Revit.DB.View;
using RVLine = Autodesk.Revit.DB.Line;

namespace WeAuto
{
	public class UVSet
	{
		public int LightingCount
		{
			get
			{
				return (RawListU.Count - 1) * (RawListV.Count -1);
			}
		}
		
		public static List<UVSet> FilterMirror(List<UVSet> uvSets, bool u, bool v)
		{
			List<UVSet> rslt = new List<UVSet>();
			foreach (UVSet uvSet in uvSets) 
			{
				if(u && !FirstlastEqual(uvSet.IndexListU))
				{
					continue;
				}

				if(v&& !FirstlastEqual(uvSet.IndexListV))
				{
					continue;
				}
				
				if(!u && FirstlastEqual(uvSet.IndexListU))
				{
					continue;
				}

				if(!v&& FirstlastEqual(uvSet.IndexListV))
				{
					continue;
				}
				
				rslt.Add(uvSet);
			}
			return rslt;
		}
		
		static bool FirstlastEqual(List<int> nums)
		{
			return nums[0] == nums[nums.Count -1];
		}
		
		public List<UV> List = new List<UV>();
		public override string ToString()
		{
			string tx = string.Empty;
			tx += IndexListU[0].ToString();
			for(int i=1;i<RawListU.Count;i++)
			{
				tx += "-" + IndexListU[i].ToString();
			}
			tx += ";\t\t";
			
			tx += IndexListV[0].ToString();
			for(int i=1;i<RawListV.Count;i++)
			{
				tx += "-" + IndexListV[i].ToString();
			}
			return tx;				
		}
		
		public void Update()
		{
			IndexListU.Add(RawListU[0]);
			for(int i=1;i<RawListU.Count;i++)
			{
				IndexListU.Add(RawListU[i] - RawListU[i-1]);
			}

			IndexListV.Add(RawListV[0]);
			for(int i=1;i<RawListV.Count;i++)
			{
				IndexListV.Add(RawListV[i] - RawListV[i-1]);
			}				
		}
		
		public List<int> RawListU = new List<int>();
		public List<int> RawListV = new List<int>();
		
		public List<int> IndexListU = new List<int>();
		public List<int> IndexListV = new List<int>();		
	}
	
	/// <summary>
	/// Description of DrawUtils.
	/// </summary>
	public class DrawUtils
	{
        public static XYZ ToXYZ(Point3d pnt)
        {
            return new XYZ(ToFt(pnt.X), ToFt(pnt.Y), ToFt(pnt.Z));
        }
        
        public static RVLine ToRVLine(RHLine ln)
        {
        	return RVLine.CreateBound(ToXYZ(ln.PointAtStart), ToXYZ(ln.PointAtEnd));
        }

        public static Vector3d ToVector3d(XYZ pnt)
        {
            return new Vector3d(ToMM(pnt.X), ToMM(pnt.Y), 0);
        }

        public static RHCurve ToRHCurve(RVCurve crv)
        {
            RVLine ln = crv as RVLine;
            if (null != ln)
            {
                return ToWLine(ln);
            }
            RVArc arc = crv as RVArc;
            if (null != arc)
            {
                // TMP
                Point3d start = ToPoint3d(crv.GetEndPoint(0));
                Point3d end = ToPoint3d(crv.GetEndPoint(1));

                return new RHLine(start, end);
            }
            return null;
        }

        public static RHLine ToWLine(RVLine ln)
        {
            Point3d start = ToPoint3d(ln.GetEndPoint(0));
            Point3d end = ToPoint3d(ln.GetEndPoint(1));

            return new RHLine(start, end);
        }

        public static RHArc ToWArc(RVArc ac)
        {
            throw new NotImplementedException();
        }

        public static double ToFt(double mm)
        {
            return mm * 0.0032808398950131;
        }

        public static double ToMM(double feet)
        {
            return feet * 304.8;
        }
		
		static bool AlmostEqual(double p0, double p1)
		{
			if(Math.Abs(p1-p0) < 0.000001)
			{
				return true;
			}
			return false;
		}
		
		public static void DrawCrvs(PictureBox pic, List<ColorLine> lns)
		{
            double xMin = double.MaxValue;
            double xMax = double.MinValue;
            double yMin = double.MaxValue;
            double yMax = double.MinValue;

            foreach (ColorLine crv in lns)
            {
            	FindMaxMin(crv.Ln.GetEndPoint(0), ref xMin, ref xMax, ref yMin, ref yMax);
                FindMaxMin(crv.Ln.GetEndPoint(1), ref xMin, ref xMax, ref yMin, ref yMax);
            }

            DrawData dd = new DrawData();

            double w = xMax - xMin;
            double h = yMax - yMin;
			
            if(w>h)
            {
            	dd.ratio = pic.Width / w * 0.8;
            }
            else
            {
            	dd.ratio = pic.Height / h * 0.8;
            }
            dd.xMin = xMin;
            dd.yMin = yMin;

            Bitmap bm = new Bitmap(pic.Width, pic.Height);
            Graphics g = Graphics.FromImage(bm);

            foreach (ColorLine crv in lns)
            {
                PointF p0 = GetPointF(crv.Ln.GetEndPoint(0), dd.ratio, xMin, yMin);
                PointF p1 = GetPointF(crv.Ln.GetEndPoint(1), dd.ratio, xMin, yMin);
                g.DrawLine(crv.ColorPen, p0, p1);

                PointF testP = new PointF(p0.X + (p1.X - p0.X) * 0.7f, p0.Y + (p1.Y - p0.Y) * 0.7f);
            }
			bm.RotateFlip(RotateFlipType.Rotate180FlipX);
            pic.Image = bm;
		}
		
		public DrawUtils()
		{
		}
		
        public static void FindMaxMin(XYZ xy, ref double xMin, ref double xMax, ref double yMin, ref double yMax)
        {
            if (xy.X < xMin)
            {
                xMin = xy.X;
            }
            if (xy.X > xMax)
            {
                xMax = xy.X;
            }
            if (xy.Y < yMin)
            {
                yMin = xy.Y;
            }
            if (xy.Y > yMax)
            {
                yMax = xy.Y;
            }
        }

        public static System.Drawing.PointF GetPointF(XYZ xy, double ratio, double xMin, double yMin)
        {
            double x = (xy.X - xMin) * ratio;
            double y = (xy.Y - yMin) * ratio;

            return new System.Drawing.PointF((float)x, (float)y);
        }
        
        public static Point3d ToPoint3d(XYZ pnt)
        {
            return new Point3d(ToMM(pnt.X), ToMM(pnt.Y), 0);
        }
	}
	
    public class ColorLine
    {
        public Pen ColorPen { get; private set; } 
        public RVLine Ln { get; private set; }
        public ColorLine(RVLine ln)
        {
            Ln = ln;
            ColorPen = Pens.Blue;
        }

        public ColorLine(RVLine ln, Pen p)
        {
            Ln = ln;
            ColorPen = p;
        }
    }	
    
    public struct DrawData
    {
        public double ratio;
        public double xMin;
        public double yMin;
        public Pen p;
    }    
}
