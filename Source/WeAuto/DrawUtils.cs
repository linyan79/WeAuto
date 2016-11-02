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
using Autodesk.Revit.DB.Architecture;
using Autodesk.Revit.UI.Selection;
using RApp = Autodesk.Revit.ApplicationServices.Application;
using RDoc = Autodesk.Revit.DB.Document;
using RHArc = Autodesk.Revit.DB.Arc;
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
		
		public static List<UVSet> FilterMirrorSingle(List<UVSet> uvSets, bool u, bool v)
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
				
				if(uvSet.IndexListU.Count == 1 || uvSet.IndexListV.Count == 1)
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
		
		public UV DistanceUV;
		public List<UV> List = new List<UV>();
		
		public override string ToString()
		{
			string tx = string.Empty;
			tx += IndexListU[0].ToString();
			for(int i=1;i<IndexListU.Count;i++)
			{
				tx += "-" + IndexListU[i].ToString();
			}
			
			int emptySpaceCount = 15 - tx.Length;
			string emptySpace = new String(' ', emptySpaceCount);
			
			tx += emptySpace + "\t";
			
			tx += IndexListV[0].ToString();
			for(int i=1;i<IndexListV.Count;i++)
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
	
	public class DimData
	{
		public Line LocLn;
		public List<Line> RefLns = new List<Line>();
		
		public bool IsEmpty
		{
			get
			{
				return LocLn == null || RefLns.Count == 0;
			}
		}
	}
	
	public class RoomData
	{
		public DimData DimH;
		public DimData DimV;
		
		public bool HasInRooms
		{
			get
			{
				return null != InRmList;
			}
		}
		public List<InnerRoom> InRmList;
		
		public RDoc m_doc{get;set;}
		public RView m_thisView{get;set;}
		public Transform m_trf{get;set;}
		public Room m_rm{get;set;}
		
		public double Width 
		{
			get
			{
				return BBx.Max.U - BBx.Min.U;
			}
		}
		public double Depth 
		{
			get
			{
				return BBx.Max.V - BBx.Min.V;
			}
		}
		
		public double LayoutWidth 
		{
			get
			{
				return LayoutBBx.Max.U - LayoutBBx.Min.U;
			}
		}
		public double LayoutDepth 
		{
			get
			{
				return LayoutBBx.Max.V - LayoutBBx.Min.V;
			}
		}		
		
		public List<UV> desks = null;
		
		public BoundingBoxUV BBx {get;set;}
		public BoundingBoxUV LayoutBBx {get;set;}
		
		List<Curve> m_ExBorder;
		public List<Curve> ExBorder
		{
			get
			{
				return m_ExBorder;
			}
			set
			{
				m_ExBorder = value;
				ExBorderPLs = Convert(m_ExBorder);
			}
		}
		public List<Line> ExBorderPLs;
		
		public List<Curve> AllBorder;
		
		public List<Line> InternalWalls;
		
		public List<List<Line>> v0Lines = new List<List<Line>>();
		public List<List<Line>> h0Lines = new List<List<Line>>();
		public List<List<Line>> v1Lines = new List<List<Line>>();
		public List<List<Line>> h1Lines = new List<List<Line>>();
		
		public Line vLn0;
		public Line vLn1;
		public Line hLn0;
		public Line hLn1;
		
		public int m_index_vLn0 = 0;
		public int m_index_vLn1 = 0;
		public int m_index_hLn0 = 0;
		public int m_index_hLn1 = 0;
		
		
		
		/// <summary>
		/// 
		/// </summary>
		/// <param name="dir">0, left; 1, right; 2, down; 3, up</param>
		public void NextAndUpdate(int dir, bool add)
		{
			switch (dir) 
			{
				case 0:
					if(add)
					{
						m_index_vLn0++;
					}
					else
					{
						m_index_vLn0--;
					}
					m_index_vLn0 = GetObj(v0Lines, m_index_vLn0, out vLn0);
					break;
				case 1:
					if(add)
					{
						m_index_vLn1++;
					}
					else
					{
						m_index_vLn1--;
					}
					m_index_vLn1 = GetObj(v1Lines, m_index_vLn1, out vLn1);						
					break;
				case 2:
					if(add)
					{
						m_index_hLn0++;
					}
					else
					{
						m_index_hLn0--;
					}
					m_index_hLn0 = GetObj(h0Lines, m_index_hLn0, out hLn0);						
					break;
				default:
					if(add)
					{
						m_index_hLn1++;
					}
					else
					{
						m_index_hLn1--;
					}
					m_index_hLn1 = GetObj(h1Lines, m_index_hLn1, out hLn1);						
					break;
			}
			Update();
		}
		
		public static int GetObj<T>(IList<List<T>> list, int index, out T obj)
		{
			int rslt = index;
			if(index > list.Count - 1)
			{
				rslt = 0;
			}
			else if(index <0)
			{
				rslt = list.Count - 1;
			}
			obj = list[rslt][0];
			return rslt;
		}
		
		public void Update()
		{	
 			double left = vLn0.GetEndPoint(0).X;
 			double right = vLn1.GetEndPoint(0).X;
 			double top = hLn0.GetEndPoint(0).Y;
 			double bottom = hLn1.GetEndPoint(0).Y;
 			
 			LayoutBBx = new BoundingBoxUV(left, bottom, right, top);			
		}
		
		public static List<Line> Convert(List<Curve> crvs)
		{
			List<Line> clns = new List<Line>();
			foreach (Curve crv in crvs) 
			{
				Line ln = crv as Line;
				if(ln != null)
				{
					clns.Add(ln);
				}
				else
				{
					if(crv.Length > 1)
					{
						clns.AddRange(Tessellate(crv));
					}	
					else
					{
						Line lnb = Line.CreateBound(crv.GetEndPoint(0), crv.GetEndPoint(1));
						clns.Add(lnb);
					}
				}
			}
			return clns;
		}
		
		public static List<Line> Tessellate(Curve crv)
		{
			int count = (int)(crv.Length / 0.5);
			double seg = 1.0 / (double)count;
			
			List<Line> lns = new List<Line>();
			
			for(int i = 0; i<count; i++)
			{
				XYZ pnt0 = crv.Evaluate(i * seg, true);
				XYZ pnt1 = crv.Evaluate((i+1) * seg, true);
				Line ln = Line.CreateBound(pnt0, pnt1);
				lns.Add(ln);
			}
			
			return lns;
		}
	}
	
	/// <summary>
	/// Description of DrawUtils.
	/// </summary>
	public class DrawUtils
	{

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
			
            if(w/h > (double)pic.Width / (double)pic.Height)
            {
            	dd.ratio = pic.Width / w * 0.8;
            }
            else
            {
            	dd.ratio = pic.Height / h * 0.8;
            }
            dd.xMin = xMin;
            dd.yMin = yMin;

            int bmW = (int)(w*dd.ratio / 0.8);
            int bmH = (int)(h*dd.ratio / 0.8);
            
            dd.marginW = (float)(bmW * 0.1);
            dd.marginH = (float)(bmH * 0.1);
            
            Bitmap bm = new Bitmap(bmW, bmH);
            Graphics g = Graphics.FromImage(bm);

            foreach (ColorLine crv in lns)
            {
                PointF p0 = GetPointF(crv.Ln.GetEndPoint(0), dd);
                PointF p1 = GetPointF(crv.Ln.GetEndPoint(1), dd);
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

        public static System.Drawing.PointF GetPointF(XYZ xy, DrawData dd) // double ratio, double xMin, double yMin, float marginW, float marginH)
        {
            double x = (xy.X - dd.xMin) * dd.ratio;
            double y = (xy.Y - dd.yMin) * dd.ratio;

            return new System.Drawing.PointF((float)x + dd.marginW, (float)y + dd.marginH);
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
        public float marginW;
        public float marginH;
    }   

	class CategorySelFilter : ISelectionFilter
	{	
		Document m_doc;
		BuiltInCategory m_bCat;
		
		public CategorySelFilter(Document doc, BuiltInCategory bCat)
		{
			m_doc = doc;
			m_bCat = bCat;
		}
		
		public bool AllowElement(Element elem)
		{
			if(elem.Category.Id.IntegerValue == (int)m_bCat)
			{
				return true;
			}
			
			return false;
		}
		
		public bool AllowReference(Reference reference, XYZ position)
		{
			Element elem = m_doc.GetElement(reference);
			return AllowElement(elem);
		}
	}    
}
