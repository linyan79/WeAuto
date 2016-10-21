/*
 * Created by SharpDevelop.
 * User: lyan
 * Date: 8/24/2016
 * Time: 9:48 AM
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

using Autodesk.Revit.DB;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Architecture;
using Autodesk.Revit.DB.PointClouds;
using Autodesk.Revit.DB.Structure;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using Autodesk.Revit.UI.Selection;
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

using RVTrf = Autodesk.Revit.DB.Transform;

namespace WeAuto
{
    [Autodesk.Revit.Attributes.Transaction(Autodesk.Revit.Attributes.TransactionMode.Manual)]
    [Autodesk.Revit.DB.Macros.AddInId("2D9E4A71-9796-4F71-8AB6-C08404D4C756")]
	public partial class ThisApplication
	{
		private void Module_Startup(object sender, EventArgs e)
		{

		}

		private void Module_Shutdown(object sender, EventArgs e)
		{

		}

		#region Revit Macros generated code
		private void InternalStartup()
		{
			this.Startup += new System.EventHandler(Module_Startup);
			this.Shutdown += new System.EventHandler(Module_Shutdown);
		}
		#endregion
		
		
		public void AutoLt01()
		{
			RApp app = Application;
			UIDocument uiDoc = ActiveUIDocument;
			RDoc doc = ActiveUIDocument.Document;
			RView thisView = uiDoc.ActiveGraphicalView;
			
			Transaction trans = new Transaction(doc, "AutoLt01");

			CategorySelFilter rmFilter = new CategorySelFilter(doc, BuiltInCategory.OST_Rooms);
			Reference elemRef = uiDoc.Selection.PickObject(ObjectType.Element, rmFilter, "Pick a Room");
			Room rm = doc.GetElement(elemRef) as Room;
			
			SpatialElementBoundaryOptions spOpt = new SpatialElementBoundaryOptions();
			
			try
			{
				double rotate = thisView.RightDirection.AngleOnPlaneTo(XYZ.BasisX, XYZ.BasisZ);
				RVTrf trf = RVTrf.CreateRotation(XYZ.BasisZ, rotate);
				RVTrf inTrf = trf.Inverse;
				
				IList<IList<RMBD>> bdSetSet = rm.GetBoundarySegments(spOpt);
				IList<RMBD> outBdSet = null;
				double maxLen = 0;
				List<RVLine> allBorders = new List<RVLine>();
	 			foreach (IList<RMBD> bdSet in bdSetSet) 
	 			{
	 				double bdLen = 0;
	 				foreach (RMBD bd in bdSet) 
	 				{
	 					allBorders.Add(GenLine(bd.Curve, trf));
	 					bdLen += bd.Curve.Length;
	 				}
	 				
	 				if(bdLen > maxLen)
	 				{
	 					maxLen = bdLen;
	 					outBdSet = bdSet;
	 				}
	 			}
	 			
	 			List<RHLine> lns = new List<LineCurve>();
	 			List<RVLine> borders = new List<RVLine>();

	 			foreach (RMBD rmBd in outBdSet) 
	 			{
	 				borders.Add(GenLine(rmBd.Curve, trf));
	 				
	 				RVLine ln = rmBd.Curve as RVLine;
	 				
	 				if(null == ln)
	 				{
	 					continue;
	 				}
	 				ln = GenLine(ln, trf);
	 				
	 				if(null == rmBd.Element)
	 				{
	 					continue;
	 				}
	 				
	 				Category cat = rmBd.Element.Category;
	 				if(null == cat)
	 				{
	 					continue;
	 				}
	 				
	 				if(cat.Id.IntegerValue != (int)BuiltInCategory.OST_Walls &&
	 				  cat.Id.IntegerValue != (int)BuiltInCategory.OST_RvtLinks &&
	 				  cat.Id.IntegerValue != (int)BuiltInCategory.OST_RoomSeparationLines)
	 				{
	 					continue;
	 				}
	 				
	 				lns.Add(DrawUtils.ToWLine(ln));
	 			}
	 			
	 			Dictionary<int, List<RHLine>> v0LinesDict = new Dictionary<int, List<LineCurve>>();
	 			Dictionary<int, List<RHLine>> h0LinesDict = new Dictionary<int, List<LineCurve>>();
	 			Dictionary<int, List<RHLine>> v1LinesDict = new Dictionary<int, List<LineCurve>>();
	 			Dictionary<int, List<RHLine>> h1LinesDict = new Dictionary<int, List<LineCurve>>();
	 			foreach (RHLine ln in lns) 
	 			{
	 				if(AddLnDict(h0LinesDict, 0, ln))
	 					continue;
	 				if(AddLnDict(v0LinesDict, 1, ln))
	 					continue;
	 				if(AddLnDict(h1LinesDict, 2, ln))
	 					continue;
	 				if(AddLnDict(v1LinesDict, 3, ln))
	 					continue;
	 			}
	 			
	 			RHLine vLn0 = GetLn(v0LinesDict, 1);
	 			RHLine vLn1 = GetLn(v1LinesDict, 3);
	 			RHLine hLn0 = GetLn(h0LinesDict, 0);
	 			RHLine hLn1 = GetLn(h1LinesDict, 2);
	 			
	 			double left = vLn0.PointAtStart.X;
	 			double right = vLn1.PointAtStart.X;
	 			double top = hLn0.PointAtStart.Y;
	 			double bottom = hLn1.PointAtStart.Y;
	 			
	 			BoundingBoxUV bdBx = new BoundingBoxUV(left, bottom, right, top);
	 			
 				double w = right - left;
 				double h = top - bottom;
 				
 				List<RVLine> internalWalls = FindInternalWalls(rm, thisView, trf);
 				allBorders.AddRange(internalWalls);
 				
 				List<UV> divs = DivLightings(w, h);
 				List<RVLine> deskOutlines = new List<RVLine>();
 				List<UV> desks = GetRmDesks(rm, thisView, deskOutlines, trf);
 				List<UVSet> uvSets = FindLightings(doc, divs, bdBx, thisView.GenLevel, desks.Count);
 				
 				List<UVSet> mySets = FindBestUVSet(uvSets, desks, allBorders, rm, trf);
 				
 				Dlg dlg = new Dlg();
 				if(mySets.Count > 1)
 				{
	 				dlg.m_rawSets = mySets;
	 				dlg.m_outlns = allBorders;
	 				dlg.m_deskslns = deskOutlines;
	 				
	 				if(DialogResult.Cancel == dlg.ShowDialog())
	 				{
	 					return;
	 				}
 				}
	 			
	 			if(trans.Start() == TransactionStatus.Started)
	 			{
//	 				doc.Create.NewDetailCurve(thisView, ToRVLine(vLn0));
//	 				doc.Create.NewDetailCurve(thisView, ToRVLine(vLn1));
//	 				doc.Create.NewDetailCurve(thisView, ToRVLine(hLn0));
//	 				doc.Create.NewDetailCurve(thisView, ToRVLine(hLn1));
	 				
	 				UVSet uvSet = mySets[0];
	 				if(mySets.Count > 1)
	 				{
	 					uvSet = dlg.m_sets[dlg.index];
	 				}
	 				GenLightings(doc, uvSet.List, thisView.GenLevel, inTrf, -rotate);
					
	 				trans.Commit();
	 			}
			}
			catch(Exception ex)
			{
				MessageBox.Show(ex.ToString());
			}
		}
		
		static RVLine GenLine(RVCurve crv, RVTrf trf)
		{
			XYZ p0 = crv.GetEndPoint(0);
			XYZ p1 = crv.GetEndPoint(1);
			
			return RVLine.CreateBound(GenXYZ(p0, trf), GenXYZ(p1, trf));
		}
		
		static XYZ GenXYZ(XYZ pnt, RVTrf trf)
		{
			return trf.OfPoint(pnt);
		}
		
		class Desk
		{
			public UV Position {get; private set;}
			public bool IsGhost{get;private set;}
			
		}
		
		static List<RVLine> FindInternalWalls(Room rm, RView thisView, RVTrf trf)
		{
			List<RVLine> rslt = new List<RVLine>();
			
			RDoc doc = rm.Document;
			FilteredElementCollector cll = new FilteredElementCollector(doc, thisView.Id);
			IList<Element> walls = 
				cll.OfCategory(BuiltInCategory.OST_Walls).OfClass(typeof(Wall)).ToElements();
			
			foreach (Wall wa in walls) 
			{
				Parameter pRmBd = wa.get_Parameter(BuiltInParameter.WALL_ATTR_ROOM_BOUNDING);
				if(pRmBd.AsInteger() != 0)
				{
					continue;
				}
				
				LocationCurve locCrv = wa.Location as LocationCurve;
				RVLine ln = locCrv.Curve as RVLine;
				if(null == ln)
				{
					continue;
				}
				XYZ pnt0 = ln.Evaluate(0.3, true);
				XYZ pnt1 = ln.Evaluate(0.7, true);
				
				if(rm.IsPointInRoom(pnt0) || rm.IsPointInRoom(pnt1))
				{
					ln = GenLine(ln, trf);
					rslt.Add(ln);
				}
			}
			
			return rslt;
		}
		
		static void GenLightings(RDoc doc, List<UV> set, Level lv, RVTrf trf, double angle)
		{
			FilteredElementCollector cll = new FilteredElementCollector(doc);
			IList<Element> ltTyps = 
				cll.OfCategory(BuiltInCategory.OST_LightingFixtures).WhereElementIsElementType().ToElements();
			FamilySymbol ltSym = null;
			foreach (FamilySymbol sym in ltTyps) 
			{
				if(sym.FamilyName == "Pendant" && sym.Name == "Generic")
				{
					ltSym = sym;
					break;
				}
			}
			
			List<ElementId> ids = new List<ElementId>();
			foreach (UV uv in set) 
			{
				XYZ pos = new XYZ(DrawUtils.ToFt(uv.U), DrawUtils.ToFt(uv.V), 0);
				pos = trf.OfPoint(pos);
				RVLine axis = RVLine.CreateBound(pos, pos + XYZ.BasisZ);
				FamilyInstance lt = doc.Create.NewFamilyInstance(pos, ltSym, lv, StructuralType.NonStructural);	
				ElementTransformUtils.RotateElement(doc, lt.Id, axis, angle);
				ids.Add(lt.Id);
			}
			
			// doc.Create.NewGroup(ids);
		}
		
		static void Div(double w, out int minNum, out int maxNum)
		{
			double max = 2500;
			double min = 1400;
			
			minNum = (int)Math.Ceiling(w / (max * 0.5));
			maxNum = (int)Math.Floor(w / (min * 0.5));
			
			if(minNum < 2)
			{
				minNum = 2;
			}
			if(maxNum < minNum)
			{
				int tmp = maxNum;
				maxNum = minNum;
				minNum = tmp;
			}
			
			if(w < min * 2)
			{
				minNum = 2;
			}
		}
		
		static List<UVSet> FindBestUVSet(List<UVSet> uvSets, List<UV> desks, 
		                                 List<RVLine> allBorders, Room rm, RVTrf trf)
		{
			int index = 0;
			int minIndex = 0;
			double minStd = double.MaxValue;
			
			RVTrf inTrf = trf.Inverse;
			
			SortedList<double, UVSet> sorted = new SortedList<double, UVSet>();
			
			List<RHLine> blockLns = new List<LineCurve>();
			foreach (RVLine rLn in allBorders) 
			{
				blockLns.Add(DrawUtils.ToWLine(rLn));
			}
			
			foreach (UVSet uvSet in uvSets) 
			{
				List<UV> uvList = new List<UV>();
				
				foreach (UV uv in uvSet.List) 
				{
					bool tooClose = false;
					Point3d testPnt = new Point3d(uv.U, uv.V, 0);
					
					foreach (RHLine ln in blockLns) 
					{
						if(ln.Line.DistanceTo(testPnt, true) < 300)
						{
							tooClose = true;
							break;
						}
					}	
					
					XYZ testXYZ = DrawUtils.ToXYZ(testPnt) + XYZ.BasisZ * rm.Level.ProjectElevation;
					testXYZ = inTrf.OfPoint(testXYZ);
					if(!rm.IsPointInRoom(testXYZ))
					{
						continue;
					}
					
					if(!tooClose)
					{
						uvList.Add(uv);
					}
				}
				
				uvSet.List = uvList;
				
				List<double> distList = new List<double>();
				bool skip = false;
				foreach (UV desk in desks) 
				{
					double shortest = ShortDist(desk, uvSet);
					if(shortest > 1500)
					{
						skip = true;
						break;
					}
					distList.Add(shortest);
				}
				
				if(skip)
				{
					index++;
					continue;
				}
				
				double std = uvList.Count * 10000 + GetAvg(distList);
				if(std < minStd)
				{
					minStd = std;
					minIndex = index;
				}
				
				while (true) 
				{
					if(sorted.ContainsKey(std))
					{
						std += 0.001;
					}
					else
					{
						sorted.Add(std, uvSet);
						break;
					}
				}
				
				index++;
			}
			
			List<UVSet> sortedUVSet = new List<UVSet>(sorted.Values);
			
			return sortedUVSet;
		}
		
		public static double GetAvg(List<double> data)
		{
	       	double dSum =0;  
	       	double dAvg =0;  
	         
	  
	       	for (int i = 0; i < data.Count; i++)  
	       	{  
	       		double dTemp = data[i];
	           	dSum += dTemp;              
	       	}  
	  
	       	//平均值  
	       	dAvg = dSum / data.Count;  	
			return dAvg;	       	
		}
		
	   	public static double GetStd(List<double> dataList)  
   		{    
	       	double dSum =0;  
	       	double dAvg =0;  
	       	double dAvgSum =0;  
	       	double dStd =0;  
	         
	  
	       	for (int i = 0; i < dataList.Count; i++)  
	       	{  
	       		double dTemp = dataList[i];
	           	dSum += dTemp;              
	       	}  
	  
	       	//平均值  
	       	dAvg = dSum / dataList.Count;  
	  
	       	for (int i = 0; i < dataList.Count; i++)  
	       	{  
	          	double dTemp = dataList[i];
	           	dAvgSum += (dTemp - dAvg) * (dTemp - dAvg);  
	       	}  
	  
	       	dAvgSum = dAvgSum / dataList.Count;  
	  
	       	//方差  
	       	dStd = Math.Sqrt(dAvgSum);  
			return dStd;
   		}  
		
		static double ShortDist(UV uv, UVSet uvSet)
		{
			double shortest = double.MaxValue;
			foreach (UV test in uvSet.List) 
			{
				double dist = test.DistanceTo(uv);
				if(dist < shortest)
				{
					shortest = dist;
				}
			}
			return shortest;
		}
		
		static List<UV> DivLightings(double w, double h)
		{			
			int aboutWNumMin = 0;
			int aboutWNumMax = 0;
			int aboutHNumMin = 0;
			int aboutHNumMax = 0;
			
			Div(w, out aboutWNumMin, out aboutWNumMax);
			Div(h, out aboutHNumMin, out aboutHNumMax);
			
			List<UV> rslt = new List<UV>();
			for(int i = aboutWNumMin; i <= aboutWNumMax; i++)
			{
				double halfWTest = w / i;
				for(int j = aboutHNumMin; j <= aboutHNumMax; j++)
				{
					double halfHTest = h / j;
					double diff = Math.Abs(halfHTest - halfWTest);
					if(diff / (halfHTest + halfWTest) < 0.3 ||
					   (i == 1 || j == 1))
					{
						rslt.Add(new UV(i, j));
					}
				}				
			}
			
			return rslt;
		}
		
		static List<UV> GetRmDesks(Room rm, RView thisView, List<RVLine> lns, RVTrf vTrf)
		{
			List<UV> rslt = new List<UV>();
			RDoc doc = rm.Document;
			FilteredElementCollector cll = new FilteredElementCollector(doc, thisView.Id);
			IList<Element> desks = 
				cll.OfCategory(BuiltInCategory.OST_FurnitureSystems).
				OfClass(typeof(FamilyInstance)).WhereElementIsNotElementType().ToElements();

			double deskW = 44.0/12.0;
			double deskD = 24.0/12.0;
			
			foreach (FamilyInstance desk in desks) 
			{
				if(desk.Symbol.FamilyName == "1_Person-Office-Desk")
				{
					if(null == desk.Room || desk.Room.Id.IntegerValue != rm.Id.IntegerValue)
					{
						continue;
					}
					
					Parameter paramGhost = desk.LookupParameter("ShowGhosted");
					int showGhosted = paramGhost.AsInteger();
					if(showGhosted > 0)
					{
						//continue;
					}
					
					LocationPoint locPnt = desk.Location as LocationPoint;
					XYZ pnt = locPnt.Point;		
					pnt = vTrf.OfPoint(pnt);
					
					Autodesk.Revit.DB.Transform trf = desk.GetTransform();
					XYZ vx = desk.HandOrientation; //trf.OfVector(XYZ.BasisX);
					XYZ vy = desk.FacingOrientation; //trf.OfVector(-XYZ.BasisY);
					
					if(desk.FacingFlipped)
					{
						vx = -vx;
					}
					if(desk.HandFlipped)
					{
						vy = -vy;
					}
					
					vx = vTrf.OfVector(vx);
					vy = vTrf.OfVector(vy);
					
					XYZ midPnt = pnt - vy * deskD * 0.5;
					UV uv = new UV(DrawUtils.ToMM(midPnt.X), DrawUtils.ToMM(midPnt.Y));
					rslt.Add(uv);
					
					XYZ pnt2 = pnt - vy * deskD;
					
					XYZ v00 = pnt - vx * deskW * 0.5;
					XYZ v01 = pnt + vx * deskW * 0.5;
					XYZ v10 = pnt2 - vx * deskW * 0.5;
					XYZ v11 = pnt2 + vx * deskW * 0.5;
					
					RVLine ln0 = RVLine.CreateBound(v00, v01);
					RVLine ln1 = RVLine.CreateBound(v10, v11);
					RVLine ln2 = RVLine.CreateBound(v00, v10);
					RVLine ln3 = RVLine.CreateBound(v01, v11);
					
					lns.Add(ln0);
					lns.Add(ln1);
					lns.Add(ln2);
					lns.Add(ln3);
				}
			}			
			return rslt;
		}
		
		static List<UVSet> FindLightings(RDoc doc, List<UV> uvs, BoundingBoxUV bdBx, Level lv, int deskCount)
		{		
			List<UVSet> uvSets = new List<UVSet>();
			
			foreach (UV uv in uvs) 
			{
	
				int uNum = (int)Math.Ceiling(uv.U / 2.0);
				int vNum = (int)Math.Ceiling(uv.V / 2.0);
				
				double halfU = (bdBx.Max.U - bdBx.Min.U) / uv.U;
				double halfV = (bdBx.Max.V - bdBx.Min.V) / uv.V;
			
				double uStartInit = bdBx.Min.U + halfU;
				double vStartInit = bdBx.Min.V + halfV;
				
				int iCount = 1;
				int jCount = 1;

				for(int m = 0; m<= iCount; m++)
				{
					for(int n = 0; n<= jCount; n++)
					{
						double uStart = uStartInit + halfU * m;
						double vStart = vStartInit + halfV * n;
						
						double u = 0, v = 0;
						List<ElementId> lts = new List<ElementId>();
						
						UVSet uvSet = new UVSet();
						for(int i = 0; i<= uNum; i++)
						{
							u = uStart + halfU * i * 2;
							if(u > bdBx.Max.U - halfU * 0.5)
							{
								break;
							}
							
							int indexU = m+1+i*2;
							if(!uvSet.RawListU.Contains(indexU))
							{
								uvSet.RawListU.Add(indexU);
							}
								
							for(int j = 0; j<= vNum; j++)
							{
								v = vStart + halfV * j * 2;
								
								if(v > bdBx.Max.V - halfV * 0.5)
								{
									break;
								}
								UV uvPos = new UV(u, v);
								
								uvSet.List.Add(uvPos);
								
								int indexV = n+1+j*2;
								if(!uvSet.RawListV.Contains(indexV))
								{
									uvSet.RawListV.Add(indexV);
								}
							}
						}
						
						uvSet.RawListU.Add((int)uv.U);
						uvSet.RawListV.Add((int)uv.V);
						uvSet.Update();
						
						int ltCount = uvSet.LightingCount;
						
						if(ltCount * 4 < deskCount)
						{
							continue;
						}
						if(ltCount * 1.5 > (double)deskCount && (deskCount != 1 && deskCount != 2))
						{
							continue;
						}
						
						uvSets.Add(uvSet);
					}
				}
			}
			return uvSets;
		}
		
		public static bool IsEven(int index)
		{
			return !IsOdd(index);
		}
		
        public static bool IsOdd(int index)
        {
            int re = 0;
            Math.DivRem(index, 2, out re);
            if (re == 0)
            {
                return false;
            }
            return true;
        }
		
		static bool AddLnDict(Dictionary<int, List<RHLine>> linesDict, int dirIndex, RHLine ln)
		{
			Vector3d dir = -Vector3d.XAxis;
			int index = 0;
			switch (dirIndex) {
				case 0:
					dir = -Vector3d.XAxis;
					index = 1;
					break;
				case 1:
					dir = - Vector3d.YAxis;
					index = 0;
					break;
				case 2:
					dir = Vector3d.XAxis;
					index = 1;
					break;
				case 3:
				default:
					dir = Vector3d.YAxis;
					index = 0;
					break;
			}
			
			if(ln.Line.Direction.IsParallelTo(dir, RhinoMath.ToRadians(5)) > 0)
			{
				List<RHLine> vLns = null;
				int y = (int)(ln.PointAtStart[index] / 10.0);
				if(!linesDict.TryGetValue(y, out vLns))
				{
					vLns = new List<LineCurve>();
					linesDict.Add(y, vLns);
				}
				vLns.Add(ln);
				return true;
			}			
			return false;
		}
		
		static RHLine GetLn(Dictionary<int, List<RHLine>> linesDict, int dirIndex)
		{
			List<List<RHLine>> lnSetSet = new List<List<LineCurve>>(linesDict.Values);
			
			double maxV = double.MinValue;
			RHLine rslt = null;
			foreach (List<RHLine> lnSet in lnSetSet) 
			{
				double len = 0;
				foreach (RHLine ln in lnSet) 
				{
					len += ln.Line.Length;
				}
				if(len < 500)
				{
					continue;
				}
				
				Point3d testPnt = lnSet[0].PointAtEnd;
				double v = 0;
				switch (dirIndex) 
				{
					case 0:
						v = testPnt.Y;
						break;
					case 1:
						v = - testPnt.X;
						break;
					case 2:
						v = - testPnt.Y;
						break;
					case 3:
					default:
						v = testPnt.X;
						break;
				}
				
				if(v > maxV)
				{
					maxV = v;
					rslt = lnSet[0];
				}
			}
			return rslt;
		}

		static RHLine GetLnOld(Dictionary<int, List<RHLine>> linesDict)
		{
			List<List<RHLine>> lnSetSet = new List<List<LineCurve>>(linesDict.Values);
			
			double maxLen = 0;
			RHLine rslt = null;
			foreach (List<RHLine> lnSet in lnSetSet) 
			{
				double len = 0;
				foreach (RHLine ln in lnSet) 
				{
					len += ln.Line.Length;
				}
				if(len > maxLen)
				{
					maxLen = len;
					rslt = lnSet[0];
				}
			}
			return rslt;
		}
	}
}
