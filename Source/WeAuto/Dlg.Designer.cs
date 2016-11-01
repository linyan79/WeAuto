﻿/*
 * Created by SharpDevelop.
 * User: lyan
 * Date: 10/20/2016
 * Time: 2:57 PM
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
namespace WeAuto
{
	partial class Dlg
	{
		/// <summary>
		/// Designer variable used to keep track of non-visual components.
		/// </summary>
		private System.ComponentModel.IContainer components = null;
		
		/// <summary>
		/// Disposes resources used by the form.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose(bool disposing)
		{
			if (disposing) {
				if (components != null) {
					components.Dispose();
				}
			}
			base.Dispose(disposing);
		}
		
		/// <summary>
		/// This method is required for Windows Forms designer support.
		/// Do not change the method contents inside the source code editor. The Forms designer might
		/// not be able to load this method if it was changed manually.
		/// </summary>
		private void InitializeComponent()
		{
			this.picBx = new System.Windows.Forms.PictureBox();
			this.btnNext = new System.Windows.Forms.Button();
			this.btnPrev = new System.Windows.Forms.Button();
			this.btnCancel = new System.Windows.Forms.Button();
			this.btnCreate = new System.Windows.Forms.Button();
			this.lb = new System.Windows.Forms.Label();
			this.listBx = new System.Windows.Forms.ListBox();
			this.chBxH = new System.Windows.Forms.CheckBox();
			this.chBxV = new System.Windows.Forms.CheckBox();
			((System.ComponentModel.ISupportInitialize)(this.picBx)).BeginInit();
			this.SuspendLayout();
			// 
			// picBx
			// 
			this.picBx.Location = new System.Drawing.Point(9, 10);
			this.picBx.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
			this.picBx.Name = "picBx";
			this.picBx.Size = new System.Drawing.Size(543, 476);
			this.picBx.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage;
			this.picBx.TabIndex = 0;
			this.picBx.TabStop = false;
			// 
			// btnNext
			// 
			this.btnNext.Location = new System.Drawing.Point(90, 493);
			this.btnNext.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
			this.btnNext.Name = "btnNext";
			this.btnNext.Size = new System.Drawing.Size(76, 19);
			this.btnNext.TabIndex = 1;
			this.btnNext.Text = "Next";
			this.btnNext.UseCompatibleTextRendering = true;
			this.btnNext.UseVisualStyleBackColor = true;
			this.btnNext.Click += new System.EventHandler(this.BtnNextClick);
			// 
			// btnPrev
			// 
			this.btnPrev.Location = new System.Drawing.Point(9, 493);
			this.btnPrev.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
			this.btnPrev.Name = "btnPrev";
			this.btnPrev.Size = new System.Drawing.Size(76, 19);
			this.btnPrev.TabIndex = 1;
			this.btnPrev.Text = "Previous";
			this.btnPrev.UseCompatibleTextRendering = true;
			this.btnPrev.UseVisualStyleBackColor = true;
			this.btnPrev.Click += new System.EventHandler(this.BtnPrevClick);
			// 
			// btnCancel
			// 
			this.btnCancel.Location = new System.Drawing.Point(770, 493);
			this.btnCancel.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
			this.btnCancel.Name = "btnCancel";
			this.btnCancel.Size = new System.Drawing.Size(76, 19);
			this.btnCancel.TabIndex = 1;
			this.btnCancel.Text = "Cancel";
			this.btnCancel.UseCompatibleTextRendering = true;
			this.btnCancel.UseVisualStyleBackColor = true;
			this.btnCancel.Click += new System.EventHandler(this.BtnCancelClick);
			// 
			// btnCreate
			// 
			this.btnCreate.Location = new System.Drawing.Point(689, 493);
			this.btnCreate.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
			this.btnCreate.Name = "btnCreate";
			this.btnCreate.Size = new System.Drawing.Size(76, 19);
			this.btnCreate.TabIndex = 1;
			this.btnCreate.Text = "Create";
			this.btnCreate.UseCompatibleTextRendering = true;
			this.btnCreate.UseVisualStyleBackColor = true;
			this.btnCreate.Click += new System.EventHandler(this.BtnCreateClick);
			// 
			// lb
			// 
			this.lb.Location = new System.Drawing.Point(180, 493);
			this.lb.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
			this.lb.Name = "lb";
			this.lb.Size = new System.Drawing.Size(505, 19);
			this.lb.TabIndex = 2;
			this.lb.UseCompatibleTextRendering = true;
			// 
			// listBx
			// 
			this.listBx.Font = new System.Drawing.Font("Microsoft Sans Serif", 7.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.listBx.FormattingEnabled = true;
			this.listBx.Location = new System.Drawing.Point(566, 63);
			this.listBx.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
			this.listBx.Name = "listBx";
			this.listBx.Size = new System.Drawing.Size(282, 394);
			this.listBx.TabIndex = 3;
			this.listBx.SelectedIndexChanged += new System.EventHandler(this.ListBxSelectedIndexChanged);
			// 
			// chBxH
			// 
			this.chBxH.Checked = true;
			this.chBxH.CheckState = System.Windows.Forms.CheckState.Checked;
			this.chBxH.Location = new System.Drawing.Point(566, 10);
			this.chBxH.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
			this.chBxH.Name = "chBxH";
			this.chBxH.Size = new System.Drawing.Size(148, 20);
			this.chBxH.TabIndex = 4;
			this.chBxH.Text = "Mirror Horizontal";
			this.chBxH.UseCompatibleTextRendering = true;
			this.chBxH.UseVisualStyleBackColor = true;
			this.chBxH.CheckedChanged += new System.EventHandler(this.ChBxHCheckedChanged);
			// 
			// chBxV
			// 
			this.chBxV.Checked = true;
			this.chBxV.CheckState = System.Windows.Forms.CheckState.Checked;
			this.chBxV.Location = new System.Drawing.Point(566, 34);
			this.chBxV.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
			this.chBxV.Name = "chBxV";
			this.chBxV.Size = new System.Drawing.Size(135, 20);
			this.chBxV.TabIndex = 4;
			this.chBxV.Text = "Mirror Vertical";
			this.chBxV.UseCompatibleTextRendering = true;
			this.chBxV.UseVisualStyleBackColor = true;
			this.chBxV.CheckedChanged += new System.EventHandler(this.ChBxVCheckedChanged);
			// 
			// Dlg
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(856, 522);
			this.Controls.Add(this.chBxV);
			this.Controls.Add(this.chBxH);
			this.Controls.Add(this.listBx);
			this.Controls.Add(this.lb);
			this.Controls.Add(this.btnPrev);
			this.Controls.Add(this.btnCreate);
			this.Controls.Add(this.btnCancel);
			this.Controls.Add(this.btnNext);
			this.Controls.Add(this.picBx);
			this.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
			this.Name = "Dlg";
			this.Text = "Dlg";
			this.Load += new System.EventHandler(this.DlgLoad);
			((System.ComponentModel.ISupportInitialize)(this.picBx)).EndInit();
			this.ResumeLayout(false);
		}
		private System.Windows.Forms.CheckBox chBxV;
		private System.Windows.Forms.CheckBox chBxH;
		private System.Windows.Forms.ListBox listBx;
		private System.Windows.Forms.Label lb;
		private System.Windows.Forms.Button btnCreate;
		private System.Windows.Forms.Button btnCancel;
		private System.Windows.Forms.Button btnPrev;
		private System.Windows.Forms.Button btnNext;
		internal System.Windows.Forms.PictureBox picBx;
	}
}
