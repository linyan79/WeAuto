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
		public List<Line> m_outlns = new List<Line>();
		public List<Line> m_deskslns = new List<Line>();
		
		void BtnNextClick(object sender, EventArgs e)
		{
			index++;
			if(index > m_sets.Count - 1)
			{
				index = 0;
			}
			
			Draw();
		}
		
		void BtnPrevClick(object sender, EventArgs e)
		{
			index--;
			if(index < 0)
			{
				index = m_sets.Count - 1;
			}
			
			Draw();
		}
		
		void Draw()
		{
			List<ColorLine> lns = new List<ColorLine>();
			
			lns.AddRange(Convert(m_outlns, Pens.Black));
			lns.AddRange(Convert(m_deskslns, Pens.Blue));
			
			if(m_sets.Count != 0)
			{
				Pen p = new Pen(System.Drawing.Color.Red);
				p.Width = 5;
				foreach (UV uv in m_sets[index].List) 
				{
					Line ln0 = Line.CreateBound(ToXYZ(uv) - XYZ.BasisX * DrawUtils.ToFt(200), 
					                           ToXYZ(uv) + XYZ.BasisX * DrawUtils.ToFt(200));
					Line ln1 = Line.CreateBound(ToXYZ(uv) - XYZ.BasisY * DrawUtils.ToFt(200), 
					                           ToXYZ(uv) + XYZ.BasisY * DrawUtils.ToFt(200));
					lns.Add(new ColorLine(ln0, p));
					lns.Add(new ColorLine(ln1, p));
				}
				lb.Text =  m_sets[index].LightingCount + " (Lighting Count) / " + 
					(m_deskslns.Count / 4).ToString() + " (Desk Count)";
				
				listBx.SelectedIndex = index;
			}
			
			DrawUtils.DrawCrvs(picBx, lns);		
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
		
		XYZ ToXYZ(UV uv)
		{
			return new XYZ(DrawUtils.ToFt(uv.U), DrawUtils.ToFt(uv.V), 0);
		}
		
		void DlgLoad(object sender, EventArgs e)
		{
			Regen();
		}
		
		void Regen()
		{
			m_sets = UVSet.FilterMirror(m_rawSets, chBxH.Checked, chBxV.Checked);
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
	}
}
