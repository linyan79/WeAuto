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
using Autodesk.Revit.DB.Architecture;
using Autodesk.Revit.DB.PointClouds;
using Autodesk.Revit.DB.Structure;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
//using Rhino;
//using Rhino.Geometry;
using RApp = Autodesk.Revit.ApplicationServices.Application;
using RDoc = Autodesk.Revit.DB.Document;
using RHArc = Autodesk.Revit.DB.Arc;
//using RHCurve = Rhino.Geometry.Curve;
//using RHLine = Rhino.Geometry.LineCurve;
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
		
		public void FindInnerRooms()
		{
			RApp app = Application;
			UIDocument uiDoc = ActiveUIDocument;
			RDoc doc = ActiveUIDocument.Document;
			RView thisView = uiDoc.ActiveGraphicalView;
			
			Transaction trans = new Transaction(doc, "AutoLt01");

			CategorySelFilter rmFilter = new CategorySelFilter(doc, BuiltInCategory.OST_Rooms);
			Reference elemRef = uiDoc.Selection.PickObject(ObjectType.Element, rmFilter, "Pick a Room");
			Room rm = doc.GetElement(elemRef) as Room;
			
			try
			{
				List<Line> border = GetBorders(rm);
				List<Line> internalWalls = FindInternalWalls(rm, thisView, RVTrf.Identity);
				
				List<InnerRoom> rslt = InnerRoom.FindInnerRooms(border, internalWalls);
	 			
	 			if(trans.Start() == TransactionStatus.Started)
	 			{
					foreach (InnerRoom inRm in rslt) 
					{
						RVArc arc = RVArc.Create(inRm.GetCenter(), 1, 0, Math.PI * 2, XYZ.BasisX, XYZ.BasisY);
						doc.Create.NewDetailCurve(thisView, arc);
					}
	 				trans.Commit();
	 			}
			}
			catch(Exception ex)
			{
				MessageBox.Show(ex.ToString());
			}
		}
		
		public static class Settings
		{
			public static double MaxDistance = DrawUtils.ToFt(3000);
			public static double MinDistance = DrawUtils.ToFt(2400);
			
			public static double MinDistanceToBlock = DrawUtils.ToFt(450);
			public static double MinDistanceToDesk = DrawUtils.ToFt(1500);
			
			public static double DimOffset = DrawUtils.ToFt(500);
		}
		
		public void AutoLt01()
		{
			RApp app = Application;
			UIDocument uiDoc = ActiveUIDocument;
			RDoc doc = ActiveUIDocument.Document;
			RView thisView = uiDoc.ActiveGraphicalView;
			ME = this;
			
			Transaction trans = new Transaction(doc, "AutoLt01");

			CategorySelFilter rmFilter = new CategorySelFilter(doc, BuiltInCategory.OST_Rooms);
			Reference elemRef = uiDoc.Selection.PickObject(ObjectType.Element, rmFilter, "Pick a Room");
			Room rm = doc.GetElement(elemRef) as Room;
			
			try
			{
				double rotate = thisView.RightDirection.AngleOnPlaneTo(XYZ.BasisX, XYZ.BasisZ);
				RVTrf trf = RVTrf.CreateRotation(XYZ.BasisZ, rotate);
				RVTrf inTrf = trf.Inverse;
 				
 				RoomData rmData = GetRmData(rm, trf, thisView);
 				rmData.m_doc = doc;
 				rmData.m_thisView = thisView;
 				rmData.m_trf = trf;
 				rmData.m_rm = rm;
 				
 				List<Curve> allBorders = new List<Curve>(rmData.ExBorder);
 				
 				rmData.InternalWalls = FindInternalWalls(rm, thisView, trf);
 				allBorders.AddRange(rmData.InternalWalls);
 				List<Line> deskOutlines = new List<Line>();
 				rmData.desks = GetRmDesks(rm, thisView, deskOutlines, trf);
 				rmData.AllBorder = allBorders;
 				
 				Dlg dlg = new Dlg();
 				dlg.m_outlns = rmData.AllBorder;
 				dlg.m_deskslns = deskOutlines;
 				dlg.m_rmData = rmData;

				UpdateDlg(dlg, rmData);
 				
 				if(DialogResult.Cancel == dlg.ShowDialog())
 				{
 					return;
 				}
	 			
	 			if(trans.Start() == TransactionStatus.Started)
	 			{
	 				UVSet uvSet = dlg.m_sets[dlg.index];
	 				GenLightings(doc, uvSet.List, rmData, thisView.GenLevel, inTrf, -rotate);
	 				GenDims(doc, rmData, thisView, inTrf, uvSet.DistanceUV);
	 				trans.Commit();
	 			}
			}
			catch(Exception ex)
			{
				MessageBox.Show(ex.ToString());
			}
		}	
		
		public static ThisApplication ME;
		
		public string LenToValue(double len)
		{
			UIDocument uiDoc = ActiveUIDocument;
			RDoc doc = ActiveUIDocument.Document;
			Units un = doc.GetUnits();
			return UnitFormatUtils.Format(un, UnitType.UT_Length, len, false, false);
		}
		
		void GenDims(RDoc doc, RoomData rmData, RView thisView, Transform inTrf, UV distUV)
		{
			if(!rmData.DimH.IsEmpty)
			{
				CreateDim(doc, thisView, inTrf, rmData.DimH, false, distUV.U);
			}
			if(!rmData.DimV.IsEmpty)
			{
				CreateDim(doc, thisView, inTrf, rmData.DimV, false, distUV.V);
			}
			
			if(rmData.HasInRooms)
			{
				foreach (InnerRoom inRm in rmData.InRmList) 
				{
					CreateDim(doc, thisView, inTrf, inRm.DimH, true, 0);
					CreateDim(doc, thisView, inTrf, inRm.DimV, true, 0);
				}
			}
		}
		
		static void CreateDim(RDoc doc, RView thisView, Transform inTrf, DimData dData, bool isInRm, double dist)
		{
			ReferenceArray refArr = new ReferenceArray();
			foreach (Line ln in dData.RefLns) 
			{
				Line segLn = GenLine(ln, inTrf);
				DetailLine dl = doc.Create.NewDetailCurve(thisView, segLn) as DetailLine;
				refArr.Append(dl.GeometryCurve.Reference);
			}
			Line locLn = GenLine(dData.LocLn, inTrf);
			Dimension dim = doc.Create.NewDimension(thisView, locLn, refArr);	

			foreach (DimensionSegment dimSeg in dim.Segments) 
			{
				if(isInRm)
				{
					dimSeg.ValueOverride = "A";
				}
				else
				{
					int count = (int)Math.Round((double)dimSeg.Value / dist);
					string prefix = "";
					if(count > 1)
					{
						prefix = count.ToString();
					}
					dimSeg.ValueOverride = prefix + "A";
				}
			}			
		}

		public static void UpdateDlg(Dlg dlg, RoomData rmData)
		{
			List<UV> divs = DivLightings(rmData.LayoutWidth, rmData.LayoutDepth);
			List<UVSet> uvSets = FindLightings(rmData.m_doc, divs, rmData);		
			List<UVSet> mySets = FindBestUVSet(uvSets, rmData);// rmData.desks, rmData.AllBorder);
			
			dlg.m_rawSets = mySets;
		}
		
		static XYZ Flat(XYZ pnt)
		{
			return new XYZ(pnt.X, pnt.Y, 0);
		}
		
		static Line Flat(Line ln)
		{
			return Line.CreateBound(Flat(ln.GetEndPoint(0)), Flat(ln.GetEndPoint(1)));
		}
		
		static Curve FlatCrv(Curve crv)
		{
			XYZ pnt = crv.GetEndPoint(0);
			XYZ mov = - XYZ.BasisZ * pnt.Z;
			Transform trf = Transform.CreateTranslation(mov);
			return crv.CreateTransformed(trf);
		}
		
		static List<Line> Flat(List<Line> lns)
		{
			List<Line> rslt = new List<Line>();
			foreach (Line ln in lns) 
			{
				rslt.Add(Flat(ln));
			}
			return rslt;
		}
		
		RoomData GetRmData(Room rm, Transform trf, RView thisView)
		{
			RoomData rmData = new RoomData();
			
			SpatialElementBoundaryOptions spOpt = new SpatialElementBoundaryOptions();
			
			IList<IList<RMBD>> bdSetSet = rm.GetBoundarySegments(spOpt);
			IList<RMBD> outBdSet = null;
			double maxLen = 0;
			List<Curve> allBorders = new List<Curve>();
 			foreach (IList<RMBD> bdSet in bdSetSet) 
 			{
 				double bdLen = 0;
 				foreach (RMBD bd in bdSet) 
 				{
 					Curve bdCrv = FlatCrv(bd.Curve);
 					allBorders.Add(bdCrv.CreateTransformed(trf));
 					bdLen += bd.Curve.Length;
 				}
 				
 				if(bdLen > maxLen)
 				{
 					maxLen = bdLen;
 					outBdSet = bdSet;
 				}
 			}
 			
 			List<Line> lns = new List<Line>();

 			foreach (RMBD rmBd in outBdSet) 
 			{
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
 				
 				lns.Add(ln);
 			}
 			
 			Dictionary<int, List<Line>> v0LinesDict = new Dictionary<int, List<Line>>();
 			Dictionary<int, List<Line>> h0LinesDict = new Dictionary<int, List<Line>>();
 			Dictionary<int, List<Line>> v1LinesDict = new Dictionary<int, List<Line>>();
 			Dictionary<int, List<Line>> h1LinesDict = new Dictionary<int, List<Line>>();
 			
 			foreach (Line ln in lns) 
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
 			
 			BoundingBoxXYZ rmBBx = rm.get_BoundingBox(thisView);
 			XYZ min = rmBBx.Min;
 			XYZ max = rmBBx.Max;
 			min = rmBBx.Transform.OfPoint(min);
 			max = rmBBx.Transform.OfVector(max);
 			min = trf.Inverse.OfPoint(min);
 			max = trf.Inverse.OfPoint(max);
 			
 			rmData.BBx = new BoundingBoxUV(min.X, min.Y, max.X, max.Y);
 			
 			rmData.vLn0 = Line.CreateBound(min + XYZ.BasisY, min);
			rmData.vLn1 = Line.CreateBound(max - XYZ.BasisY, max);
			rmData.hLn0 = Line.CreateBound(min, min + XYZ.BasisX);
			rmData.hLn1 = Line.CreateBound(max, max - XYZ.BasisX);
 			
 			if(v0LinesDict.Count == 0)
 			{
 				List<Line> tmpLns = new List<Line>();
 				tmpLns.Add(rmData.vLn0);
 				v0LinesDict.Add(0, tmpLns);
 			}
 			if(v1LinesDict.Count == 0)
 			{
 				List<Line> tmpLns = new List<Line>();
 				tmpLns.Add(rmData.vLn1);
 				v1LinesDict.Add(0, tmpLns);
 			}
 			if(h0LinesDict.Count ==0)
 			{
 				List<Line> tmpLns = new List<Line>();
 				tmpLns.Add(rmData.hLn0);
 				h0LinesDict.Add(0, tmpLns);
 			}
 			if(h1LinesDict.Count == 0)
 			{
 				List<Line> tmpLns = new List<Line>();
 				tmpLns.Add(rmData.hLn1);
 				h1LinesDict.Add(0, tmpLns);
 			}
 			
			rmData.m_index_vLn0 = GetLn(v0LinesDict, 1, ref rmData.vLn0);
			rmData.m_index_vLn1 = GetLn(v1LinesDict, 3, ref rmData.vLn1);
			rmData.m_index_hLn0 = GetLn(h0LinesDict, 0, ref rmData.hLn0);
			rmData.m_index_hLn1 = GetLn(h1LinesDict, 2, ref rmData.hLn1);
 			
			rmData.v0Lines = new List<List<Line>>(v0LinesDict.Values);
			rmData.v1Lines = new List<List<Line>>(v1LinesDict.Values);
			rmData.h0Lines = new List<List<Line>>(h0LinesDict.Values);
			rmData.h1Lines = new List<List<Line>>(h1LinesDict.Values);
			
			rmData.ExBorder = allBorders;
 			rmData.Update();
 			return rmData;
		}
		
		static List<RVLine> GetBorders(Room rm)
		{
			SpatialElementBoundaryOptions spOpt = new SpatialElementBoundaryOptions();
			
			IList<IList<RMBD>> bdSetSet = rm.GetBoundarySegments(spOpt);
			IList<RMBD> outBdSet = null;
			double maxLen = 0;
			List<RVLine> allBorders = new List<RVLine>();
 			foreach (IList<RMBD> bdSet in bdSetSet) 
 			{
 				double bdLen = 0;
 				foreach (RMBD bd in bdSet) 
 				{
 					allBorders.Add(GenLine(bd.Curve, RVTrf.Identity));
 					bdLen += bd.Curve.Length;
 				}
 				
 				if(bdLen > maxLen)
 				{
 					maxLen = bdLen;
 					outBdSet = bdSet;
 				}
 			}
 			
 			List<Line> lns = new List<Line>();
 			List<Line> borders = new List<Line>();

 			foreach (RMBD rmBd in outBdSet) 
 			{
 				borders.Add(GenLine(rmBd.Curve, RVTrf.Identity));
 				
 				RVLine ln = rmBd.Curve as RVLine;
 				
 				if(null == ln)
 				{
 					continue;
 				}
 				ln = GenLine(ln, RVTrf.Identity);
 				
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
 				
 				lns.Add(ln);
 			}
			return lns; 			
		}
		
		static RVLine GenLine(RVCurve crv, RVTrf trf)
		{
			XYZ p0 = crv.GetEndPoint(0);
			XYZ p1 = crv.GetEndPoint(1);
			
			return RVLine.CreateBound(GenXYZ(p0, trf), GenXYZ(p1, trf));
		}
		
		static XYZ GenXYZ(XYZ pnt, RVTrf trf)
		{
			XYZ rslt = trf.OfPoint(pnt);
			return new XYZ(rslt.X, rslt.Y, 0);
		}
		
		class Desk
		{
			public UV Position {get; private set;}
			public bool IsGhost{get;private set;}
			
		}
		
		static void CreateDimensions(RDoc doc, RoomData rmData, int dir)
		{
			
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
					rslt.Add(Flat(ln));
				}
			}
			
			return rslt;
		}
		
		static void GenLightings(RDoc doc, List<UV> set, RoomData rmData, Level lv, RVTrf trf, double angle)
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
				XYZ pos = new XYZ(uv.U, uv.V, 0);
				pos = trf.OfPoint(pos);
				RVLine axis = RVLine.CreateBound(pos, pos + XYZ.BasisZ);
				FamilyInstance lt = doc.Create.NewFamilyInstance(pos, ltSym, lv, StructuralType.NonStructural);	
				ElementTransformUtils.RotateElement(doc, lt.Id, axis, angle);
				ids.Add(lt.Id);
			}
			
			if(rmData.HasInRooms)
			{
				foreach (InnerRoom inRm in rmData.InRmList) 
				{
					XYZ center = inRm.GetCenter();
					center = trf.OfPoint(center);
					RVLine axis = RVLine.CreateBound(center, center + XYZ.BasisZ);
					FamilyInstance lt = doc.Create.NewFamilyInstance(center, ltSym, lv, StructuralType.NonStructural);	
					ElementTransformUtils.RotateElement(doc, lt.Id, axis, angle);
					ids.Add(lt.Id);
				}
			}
			
			// doc.Create.NewGroup(ids);
		}
		
		static void Div(double w, out int minNum, out int maxNum)
		{		
			minNum = (int)Math.Floor(w / (Settings.MaxDistance * 0.5));
			maxNum = (int)Math.Ceiling(w / (Settings.MinDistance * 0.5));
			
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
			
			if(w < Settings.MinDistance * 2)
			{
				minNum = 2;
			}
		}
		
		static List<UVSet> FindBestUVSet(List<UVSet> uvSets, RoomData rmData)
		{
			List<UV> desks = rmData.desks;
            List<Curve> blockLns = rmData.AllBorder;
			int index = 0;
			int minIndex = 0;
			double minStd = double.MaxValue;
			
			SortedList<double, UVSet> sorted = new SortedList<double, UVSet>();

			if(rmData.m_rm.Area > 120)
			{
				rmData.InRmList = InnerRoom.FindInnerRooms(rmData.ExBorderPLs, rmData.InternalWalls);
			}
			
			foreach (UVSet uvSet in uvSets) 
			{
				List<UV> uvList = new List<UV>();
				
				foreach (UV uv in uvSet.List) 
				{
					bool tooClose = false;
					XYZ testPnt = new XYZ(uv.U, uv.V, 0);
					
					foreach (Curve crv in blockLns) 
					{
						if(crv.Distance(testPnt) < Settings.MinDistanceToBlock)
						{
							tooClose = true;
							break;
						}
					}	
					
					if(tooClose)
					{
						continue;
					}
					
					bool inInternalRm = false;
					if(rmData.HasInRooms)
					{
						foreach (InnerRoom inRm in rmData.InRmList) 
						{
							if(inRm.IsInside(testPnt))
							{
								inInternalRm = true;
								break;
							}
						}
					}
					
					if(inInternalRm)
					{
						continue;
					}
					
					uvList.Add(uv);
				}
				
				uvSet.List = uvList;
				
				List<double> distList = new List<double>();
				bool skip = false;
				foreach (UV desk in desks) 
				{
					double shortest = ShortDist(desk, uvSet);
					if(shortest > Settings.MinDistanceToDesk)
					{
						skip = true;
						break;
					}
					distList.Add(shortest);
				}
				
				if(skip)
				{
					// index++;
					// continue;
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
			if(data.Count == 0)
			{
				return 0;
			}
			
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

					if((halfHTest/halfWTest < 1.3 && halfWTest/halfHTest < 1.3 )
						|| (i == 2 || j == 2))
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
					UV uv = new UV(midPnt.X, midPnt.Y);
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
		
		static List<UVSet> FindLightings(RDoc doc, List<UV> uvs, RoomData rmData)
		{		
			BoundingBoxUV bdBx = rmData.BBx;
			BoundingBoxUV lyBdBx = rmData.LayoutBBx;
			Level lv = rmData.m_thisView.GenLevel;
			int deskCount = rmData.desks.Count;
			Room rm = rmData.m_rm;
			RVTrf trf = rmData.m_trf;
			
			RVTrf inTrf = trf.Inverse;
			List<UVSet> uvSets = new List<UVSet>();
			
			foreach (UV uv in uvs) 
			{
//				int uNum = (int)Math.Ceiling(uv.U / 2.0);
//				int vNum = (int)Math.Ceiling(uv.V / 2.0);
				
				double halfU = (lyBdBx.Max.U - lyBdBx.Min.U) / uv.U;
				double halfV = (lyBdBx.Max.V - lyBdBx.Min.V) / uv.V;
			
				int uNum = (int)Math.Ceiling(rmData.Width / halfU);
				int vNum = (int)Math.Ceiling(rmData.Depth / halfV);
				
				double uStartInit = lyBdBx.Min.U + halfU;
				double vStartInit = lyBdBx.Min.V + halfV;
				
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
						for(int i = -uNum; i<= uNum; i++)
						{
							u = uStart + halfU * i * 2;
							if(u > bdBx.Max.U - halfU * 0.5 ||
							  u < bdBx.Min.U + halfU * 0.5)
							{
								continue;
							}
							
							if(i >= 0)
							{
								int indexU = m+1+i*2;
								
								if(!uvSet.RawListU.Contains(indexU) && indexU < uv.U)
								{
									uvSet.RawListU.Add(indexU);
								}
							}
								
							for(int j = -vNum; j<= vNum; j++)
							{
								v = vStart + halfV * j * 2;
								
								if(v > bdBx.Max.V - halfV * 0.5 ||
								  v < bdBx.Min.V + halfV * 0.5)
								{
									continue;
								}
								
								
								XYZ testPnt = new XYZ(u, v, 0);
								XYZ testXYZ = testPnt + XYZ.BasisZ * rm.Level.ProjectElevation;
								testXYZ = inTrf.OfPoint(testXYZ);
								
								if(j >= 0 && j < uv.V)
								{
									int indexV = n+1+j*2;
									if(!uvSet.RawListV.Contains(indexV) && indexV < uv.V)
									{
										uvSet.RawListV.Add(indexV);
									}
								}
								
								if(!rm.IsPointInRoom(testXYZ))
								{
									continue;
								}
								
								UV uvPos = new UV(u, v);
								uvSet.List.Add(uvPos);
							}
						}
						
						uvSet.RawListU.Add((int)uv.U);
						uvSet.RawListV.Add((int)uv.V);
						uvSet.Update();
						
						int ltCount = uvSet.LightingCount;
						
						if(ltCount * 4 < deskCount)
						{
							// continue;
						}
						if(ltCount * 1.2 > (double)deskCount && (deskCount != 1 && deskCount != 2))
						{
							// continue;
						}
						
						uvSet.DistanceUV = new UV(halfU, halfV);
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
		
		static bool AddLnDict(Dictionary<int, List<Line>> linesDict, int dirIndex, Line ln)
		{
			XYZ dir = -XYZ.BasisX;
			int index = 0;
			switch (dirIndex) {
				case 0:
					dir = -XYZ.BasisX;
					index = 1;
					break;
				case 1:
					dir = - XYZ.BasisY;
					index = 0;
					break;
				case 2:
					dir = XYZ.BasisX;
					index = 1;
					break;
				case 3:
				default:
					dir = XYZ.BasisY;
					index = 0;
					break;
			}
			
			if(IsParallelTo(ln.Direction, dir, ToRadians(5)) > 0)
			{
				List<Line> vLns = null;
				int y = (int)(ln.GetEndPoint(0)[index] * 10);
				if(!linesDict.TryGetValue(y, out vLns))
				{
					vLns = new List<Line>();
					linesDict.Add(y, vLns);
				}
				vLns.Add(ln);
				return true;
			}			
			return false;
		}
		
		static double ToRadians(double degree)
		{
			return degree * Math.PI / 180.0;
		}
		
		static int IsParallelTo(XYZ d0, XYZ d1, double eps)
		{
			double r = d0.AngleTo(d1);
			if(r<eps)
			{
				return 1;
			}
			if(Math.PI - r<eps)
			{
				return -1;
			}
			return 0;
		}
		
		static int GetLn(Dictionary<int, List<Line>> linesDict, int dirIndex, ref Line rsltLn)
		{
			List<List<Line>> lnSetSet = new List<List<Line>>(linesDict.Values);
			int rslt = 0;
			double maxV = double.MinValue;
			
			foreach (List<Line> lnSet in lnSetSet) 
			{
				double len = 0;
				foreach (Line ln in lnSet) 
				{
					len += ln.Length;
				}
				if(len < DrawUtils.ToFt(500))
				{
					continue;
				}
				
				XYZ testPnt = lnSet[0].GetEndPoint(1);
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
				
				if(len > maxV)
				{
					maxV = len;
					rsltLn = lnSet[0];
				}
				
				rslt++;
			}
			return rslt;
		}

		static Line GetLnOld(Dictionary<int, List<Line>> linesDict)
		{
			List<List<Line>> lnSetSet = new List<List<Line>>(linesDict.Values);
			
			double maxLen = 0;
			Line rslt = null;
			foreach (List<Line> lnSet in lnSetSet) 
			{
				double len = 0;
				foreach (Line ln in lnSet) 
				{
					len += ln.Length;
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
