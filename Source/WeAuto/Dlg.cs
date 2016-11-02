/*
 * Created by SharpDevelop.
 * User: lyan
 * Date: 10/20/2016
 * Time: 2:57 PM
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

using Autodesk.Revit.DB;

namespace WeAuto
{
	/// <summary>
	/// Description of Dlg.
	/// </summary>
	public partial class Dlg : System.Windows.Forms.Form
	{
		public Dlg()
		{
			//
			// The InitializeComponent() call is required for Windows Forms designer support.
			//
			InitializeComponent();
			
			//
			// TODO: Add constructor code after the InitializeComponent() call.
			//
		}
		
		public int index = 0;
		public List<UVSet> m_rawSets = new List<UVSet>();
		public List<UVSet> m_sets = new List<UVSet>();
		public List<Curve> m_outlns = new List<Curve>();
		public List<Line> m_deskslns = new List<Line>();
		
		public RoomData m_rmData;
		
		void BtnNextClick(object sender, EventArgs e)
		{
			index++;
			if(index > m_sets.Count - 1)
			{
				index = 0;
			}
			
			Draw();
			listBx.SelectedIndex = index;
		}
		
		void BtnPrevClick(object sender, EventArgs e)
		{
			index--;
			if(index < 0)
			{
				index = m_sets.Count - 1;
			}
			
			Draw();
			listBx.SelectedIndex = index;
		}
		
		void Draw()
		{
			List<ColorLine> lns = new List<ColorLine>();
			
			lns.AddRange(Convert(m_outlns, Pens.Black));
			lns.AddRange(Convert(m_deskslns, Pens.Blue));
			
			if(m_sets.Count != 0)
			{
				Pen p = new Pen(System.Drawing.Color.Red);
				p.Width = 4;
				foreach (UV uv in m_sets[index].List) 
				{
					AddLight(uv, lns, p);
				}

				if(m_rmData.HasInRooms)
				{
					foreach (InnerRoom inRm in  m_rmData.InRmList)
					{
						XYZ center = inRm.GetCenter();
						UV uv = new UV(center.X, center.Y);
						AddLight(uv, lns, p);
					}
				}
				UVSet thisSet = m_sets[index];
				lb.Text = thisSet.LightingCount + " (Lighting Count) / " + 
					(m_deskslns.Count / 4).ToString() + " (Desk Count)";
					
				lb2.Text = "X Distance: " + ThisApplication.ME.LenToValue(thisSet.DistanceUV.U) +
					";\t\t  Y Distance: " + ThisApplication.ME.LenToValue(thisSet.DistanceUV.V);
			}
			
			Pen p2 = new Pen(System.Drawing.Color.Purple, 2);
			{
				lns.Add(new ColorLine(m_rmData.vLn0, p2));
				lns.Add(new ColorLine(m_rmData.vLn1, p2));
				lns.Add(new ColorLine(m_rmData.hLn0, p2));
				lns.Add(new ColorLine(m_rmData.hLn1, p2));
			}
			
			GenDim();
			DrawDim(m_rmData.DimH, lns);
			DrawDim(m_rmData.DimV, lns);
			GenDrawInRmDim(m_rmData, lns, !ckBxLeft.Checked, !ckBxBottom.Checked);
			DrawUtils.DrawCrvs(picBx, lns);		
		}
		
		void AddLight(UV uv, List<ColorLine> lns, Pen p)
		{
			Line ln0 = Line.CreateBound(ToXYZ(uv) - XYZ.BasisX * DrawUtils.ToFt(200), 
			                           ToXYZ(uv) + XYZ.BasisX * DrawUtils.ToFt(200));
			Line ln1 = Line.CreateBound(ToXYZ(uv) - XYZ.BasisY * DrawUtils.ToFt(200), 
			                           ToXYZ(uv) + XYZ.BasisY * DrawUtils.ToFt(200));
			lns.Add(new ColorLine(ln0, p));
			lns.Add(new ColorLine(ln1, p));			
		}
		
		static void GenDrawInRmDim(RoomData rmData, List<ColorLine> clns, bool isLeft, bool isBottom)
		{
			if(!rmData.HasInRooms)
			{
				return;
			}
			
			foreach (InnerRoom inRm in rmData.InRmList) 
			{
				inRm.GenDim(isLeft, isBottom);
				DrawDim(inRm.DimH, clns);
				DrawDim(inRm.DimV, clns);				
			}
		}
		
		static void DrawDim(DimData dim, List<ColorLine> clns)
		{
			if(dim.IsEmpty)
			{
				return;
			}
			
			List<Line> lns = new List<Line>(dim.RefLns);
			lns.Add(dim.LocLn);
			clns.AddRange(Convert(lns, Pens.DarkOrange));
		}
		
		void GenDim()
		{
			UVSet uvSet = m_sets[index];
			
			SortedDictionary<int, double> uDict = new SortedDictionary<int, double>();
			SortedDictionary<int, double> vDict = new SortedDictionary<int, double>();
			
			foreach (UV pos in uvSet.List) 
			{
				int uNum = (int)pos.U;
				int vNum = (int)pos.V;
				
				if(!uDict.ContainsKey(uNum))
				{
					uDict.Add(uNum, pos.U);
				}
				if(!vDict.ContainsKey(vNum))
				{
					vDict.Add(vNum, pos.V);
				}
			}
			
			m_rmData.DimH = new DimData();
			m_rmData.DimV = new DimData();
			
			List<double> us = new List<double>(uDict.Values);
			List<double> vs = new List<double>(vDict.Values);
			
			us.Insert(0, m_rmData.vLn0.GetEndPoint(0).X);
			us.Add(m_rmData.vLn1.GetEndPoint(0).X);
			vs.Insert(0, m_rmData.hLn0.GetEndPoint(0).Y);
			vs.Add(m_rmData.hLn1.GetEndPoint(0).Y);
			
			double off = ThisApplication.Settings.DimOffset;
			
			if(us.Count > 1 && ckBxTopBottom.Checked)
			{
				double v0 = m_rmData.BBx.Min.V;
				double mov = - off;

				if(!ckBxBottom.Checked)
				{
					v0 = m_rmData.BBx.Max.V;
					mov = off;
				}
				XYZ hStart = new XYZ(us[0], v0 + mov, 0);
				XYZ hEnd =  new XYZ(us[us.Count - 1], v0 + mov, 0);
				Line hLn = Line.CreateBound(hStart, hEnd);	
				m_rmData.DimH.LocLn = hLn;
				
				for(int i = 0; i<us.Count; i++)
				{
					XYZ p0 = new XYZ(us[i], v0 - mov, 0);
					XYZ p1 = new XYZ(us[i], v0 + mov, 0);
					Line lnSeg = Line.CreateBound(p0, p1);
					m_rmData.DimH.RefLns.Add(lnSeg);
				}
			}
			
			if(vs.Count > 1 && ckBxLeftRight.Checked)
			{
				double u0 = m_rmData.BBx.Min.U;
				double mov = -off;
				if(!ckBxLeft.Checked)
				{
					u0 = m_rmData.BBx.Max.U;
					mov = off;				
				}
				XYZ vStart = new XYZ(u0 + mov, vs[0], 0);
				XYZ vEnd =  new XYZ(u0 + mov, vs[vs.Count - 1], 0);
				Line vLn = Line.CreateBound(vStart, vEnd);
				m_rmData.DimV.LocLn = vLn;
				
				for(int i = 0; i<vs.Count; i++)
				{
					XYZ p0 = new XYZ(u0 - mov, vs[i], 0);
					XYZ p1 = new XYZ(u0 + mov, vs[i], 0);
					Line lnSeg = Line.CreateBound(p0, p1);
					m_rmData.DimV.RefLns.Add(lnSeg);
				}
			}
			
			if(!ckBxTopBottom.Checked)
			{
				m_rmData.DimH.LocLn = null;
			}
			if(!ckBxLeftRight.Checked)
			{
				m_rmData.DimV.LocLn = null;
			}
		}
		
		static List<ColorLine> Convert(List<Line> lns, Pen p)
		{
			List<ColorLine> clns = new List<ColorLine>();
			foreach (Line ln in lns) 
			{
				clns.Add(new ColorLine(ln, p));
			}
			return clns;
		}

		static List<ColorLine> Convert(List<Curve> lns, Pen p)
		{
			List<ColorLine> clns = new List<ColorLine>();
			foreach (Curve crv in lns) 
			{
				Line ln = crv as Line;
				if(ln != null)
				{
					clns.Add(new ColorLine(ln, p));
				}
				else
				{
					if(crv.Length > 1)
					{
						Line ln0 = Line.CreateBound(crv.GetEndPoint(0), crv.Evaluate(0.25, true));
						Line ln1 = Line.CreateBound(crv.Evaluate(0.25, true), crv.Evaluate(0.5, true));
						Line ln2 = Line.CreateBound(crv.Evaluate(0.5, true), crv.Evaluate(0.75, true));
						Line ln3 = Line.CreateBound(crv.Evaluate(0.75, true), crv.GetEndPoint(1));
						clns.Add(new ColorLine(ln0, p));
						clns.Add(new ColorLine(ln1, p));
						clns.Add(new ColorLine(ln2, p));
						clns.Add(new ColorLine(ln3, p));
					}	
					else
					{
						Line lnb = Line.CreateBound(crv.GetEndPoint(0), crv.GetEndPoint(1));
						clns.Add(new ColorLine(lnb, p));
					}
				}
			}
			return clns;
		}
		
		XYZ ToXYZ(UV uv)
		{
			return new XYZ(uv.U, uv.V, 0);
		}
		
		void DlgLoad(object sender, EventArgs e)
		{
			Regen();
		}
		
		void Regen()
		{
			m_sets = UVSet.FilterMirrorSingle(m_rawSets, chBxH.Checked, chBxV.Checked);
			listBx.DataSource = m_sets;
			index = 0;
			if(m_sets.Count > 0)
			{
				listBx.SelectedIndex = 0;
				btnCreate.Enabled = true;				
			}
			else
			{
				btnCreate.Enabled = false;
			}
			Draw();
		}
		
		void BtnCreateClick(object sender, EventArgs e)
		{
			DialogResult = DialogResult.OK;
			Close();
		}
		
		void BtnCancelClick(object sender, EventArgs e)
		{
			DialogResult = DialogResult.Cancel;
			Close();
		}
		
		void ListBxSelectedIndexChanged(object sender, EventArgs e)
		{
			index = listBx.SelectedIndex;	
			Draw();
		}
		
		void ChBxHCheckedChanged(object sender, EventArgs e)
		{
			Regen();
		}
		
		void ChBxVCheckedChanged(object sender, EventArgs e)
		{
			Regen();
		}
		
		void BtnEdgUpClick(object sender, EventArgs e)
		{
			m_rmData.NextAndUpdate(2, true);
			ThisApplication.UpdateDlg(this, m_rmData);
			Regen();
		}
		
		void BtnEdgLeftClick(object sender, EventArgs e)
		{
			m_rmData.NextAndUpdate(0, true);
			ThisApplication.UpdateDlg(this, m_rmData);
			Regen();
		}
		
		void BtnEdgRightClick(object sender, EventArgs e)
		{
			m_rmData.NextAndUpdate(1, true);
			ThisApplication.UpdateDlg(this, m_rmData);
			Regen();
		}
		
		void BtnEdgDownClick(object sender, EventArgs e)
		{
			m_rmData.NextAndUpdate(3, true);
			ThisApplication.UpdateDlg(this, m_rmData);
			Regen();
		}
		
		void CkBxUpCheckedChanged(object sender, EventArgs e)
		{
			Draw();
		}
		
		void CkBxLeftCheckedChanged(object sender, EventArgs e)
		{
			Draw();
		}
	}
}
