/*
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
			this.btnEdgUp = new System.Windows.Forms.Button();
			this.btnEdgLeft = new System.Windows.Forms.Button();
			this.btnEdgDown = new System.Windows.Forms.Button();
			this.btnEdgRight = new System.Windows.Forms.Button();
			this.ckBxBottom = new System.Windows.Forms.CheckBox();
			this.ckBxLeft = new System.Windows.Forms.CheckBox();
			this.ckBxTopBottom = new System.Windows.Forms.CheckBox();
			this.ckBxLeftRight = new System.Windows.Forms.CheckBox();
			((System.ComponentModel.ISupportInitialize)(this.picBx)).BeginInit();
			this.SuspendLayout();
			// 
			// picBx
			// 
			this.picBx.BackColor = System.Drawing.Color.White;
			this.picBx.Location = new System.Drawing.Point(9, 10);
			this.picBx.Margin = new System.Windows.Forms.Padding(2);
			this.picBx.Name = "picBx";
			this.picBx.Size = new System.Drawing.Size(543, 476);
			this.picBx.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
			this.picBx.TabIndex = 0;
			this.picBx.TabStop = false;
			// 
			// btnNext
			// 
			this.btnNext.Location = new System.Drawing.Point(90, 493);
			this.btnNext.Margin = new System.Windows.Forms.Padding(2);
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
			this.btnPrev.Margin = new System.Windows.Forms.Padding(2);
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
			this.btnCancel.Margin = new System.Windows.Forms.Padding(2);
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
			this.btnCreate.Margin = new System.Windows.Forms.Padding(2);
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
			this.listBx.Location = new System.Drawing.Point(566, 35);
			this.listBx.Margin = new System.Windows.Forms.Padding(2);
			this.listBx.Name = "listBx";
			this.listBx.Size = new System.Drawing.Size(282, 277);
			this.listBx.TabIndex = 3;
			this.listBx.SelectedIndexChanged += new System.EventHandler(this.ListBxSelectedIndexChanged);
			// 
			// chBxH
			// 
			this.chBxH.Checked = true;
			this.chBxH.CheckState = System.Windows.Forms.CheckState.Checked;
			this.chBxH.Location = new System.Drawing.Point(566, 10);
			this.chBxH.Margin = new System.Windows.Forms.Padding(2);
			this.chBxH.Name = "chBxH";
			this.chBxH.Size = new System.Drawing.Size(148, 20);
			this.chBxH.TabIndex = 4;
			this.chBxH.Text = "Mirror Left & Right";
			this.chBxH.UseCompatibleTextRendering = true;
			this.chBxH.UseVisualStyleBackColor = true;
			this.chBxH.CheckedChanged += new System.EventHandler(this.ChBxHCheckedChanged);
			// 
			// chBxV
			// 
			this.chBxV.Checked = true;
			this.chBxV.CheckState = System.Windows.Forms.CheckState.Checked;
			this.chBxV.Location = new System.Drawing.Point(689, 11);
			this.chBxV.Margin = new System.Windows.Forms.Padding(2);
			this.chBxV.Name = "chBxV";
			this.chBxV.Size = new System.Drawing.Size(135, 20);
			this.chBxV.TabIndex = 4;
			this.chBxV.Text = "Mirror Top & Bottom";
			this.chBxV.UseCompatibleTextRendering = true;
			this.chBxV.UseVisualStyleBackColor = true;
			this.chBxV.CheckedChanged += new System.EventHandler(this.ChBxVCheckedChanged);
			// 
			// btnEdgUp
			// 
			this.btnEdgUp.Location = new System.Drawing.Point(648, 317);
			this.btnEdgUp.Name = "btnEdgUp";
			this.btnEdgUp.Size = new System.Drawing.Size(117, 23);
			this.btnEdgUp.TabIndex = 5;
			this.btnEdgUp.Text = "Next Up Edge";
			this.btnEdgUp.UseCompatibleTextRendering = true;
			this.btnEdgUp.UseVisualStyleBackColor = true;
			this.btnEdgUp.Click += new System.EventHandler(this.BtnEdgUpClick);
			// 
			// btnEdgLeft
			// 
			this.btnEdgLeft.Location = new System.Drawing.Point(588, 346);
			this.btnEdgLeft.Name = "btnEdgLeft";
			this.btnEdgLeft.Size = new System.Drawing.Size(117, 23);
			this.btnEdgLeft.TabIndex = 6;
			this.btnEdgLeft.Text = "Next Left Edge";
			this.btnEdgLeft.UseCompatibleTextRendering = true;
			this.btnEdgLeft.UseVisualStyleBackColor = true;
			this.btnEdgLeft.Click += new System.EventHandler(this.BtnEdgLeftClick);
			// 
			// btnEdgDown
			// 
			this.btnEdgDown.Location = new System.Drawing.Point(648, 375);
			this.btnEdgDown.Name = "btnEdgDown";
			this.btnEdgDown.Size = new System.Drawing.Size(117, 23);
			this.btnEdgDown.TabIndex = 7;
			this.btnEdgDown.Text = "Next Down Edge";
			this.btnEdgDown.UseCompatibleTextRendering = true;
			this.btnEdgDown.UseVisualStyleBackColor = true;
			this.btnEdgDown.Click += new System.EventHandler(this.BtnEdgDownClick);
			// 
			// btnEdgRight
			// 
			this.btnEdgRight.Location = new System.Drawing.Point(708, 346);
			this.btnEdgRight.Name = "btnEdgRight";
			this.btnEdgRight.Size = new System.Drawing.Size(117, 23);
			this.btnEdgRight.TabIndex = 8;
			this.btnEdgRight.Text = "Next Right Edge";
			this.btnEdgRight.UseCompatibleTextRendering = true;
			this.btnEdgRight.UseVisualStyleBackColor = true;
			this.btnEdgRight.Click += new System.EventHandler(this.BtnEdgRightClick);
			// 
			// ckBxBottom
			// 
			this.ckBxBottom.Location = new System.Drawing.Point(566, 418);
			this.ckBxBottom.Name = "ckBxBottom";
			this.ckBxBottom.Size = new System.Drawing.Size(139, 24);
			this.ckBxBottom.TabIndex = 9;
			this.ckBxBottom.Text = "Dimension Bottom";
			this.ckBxBottom.UseCompatibleTextRendering = true;
			this.ckBxBottom.UseVisualStyleBackColor = true;
			this.ckBxBottom.CheckedChanged += new System.EventHandler(this.CkBxUpCheckedChanged);
			// 
			// ckBxLeft
			// 
			this.ckBxLeft.Location = new System.Drawing.Point(566, 448);
			this.ckBxLeft.Name = "ckBxLeft";
			this.ckBxLeft.Size = new System.Drawing.Size(104, 24);
			this.ckBxLeft.TabIndex = 9;
			this.ckBxLeft.Text = "Dimension Left";
			this.ckBxLeft.UseCompatibleTextRendering = true;
			this.ckBxLeft.UseVisualStyleBackColor = true;
			this.ckBxLeft.CheckedChanged += new System.EventHandler(this.CkBxLeftCheckedChanged);
			// 
			// ckBxTopBottom
			// 
			this.ckBxTopBottom.Checked = true;
			this.ckBxTopBottom.CheckState = System.Windows.Forms.CheckState.Checked;
			this.ckBxTopBottom.Location = new System.Drawing.Point(681, 418);
			this.ckBxTopBottom.Name = "ckBxTopBottom";
			this.ckBxTopBottom.Size = new System.Drawing.Size(167, 24);
			this.ckBxTopBottom.TabIndex = 9;
			this.ckBxTopBottom.Text = "Has Top/Bottom Dimension";
			this.ckBxTopBottom.UseCompatibleTextRendering = true;
			this.ckBxTopBottom.UseVisualStyleBackColor = true;
			this.ckBxTopBottom.CheckedChanged += new System.EventHandler(this.CkBxUpCheckedChanged);
			// 
			// ckBxLeftRight
			// 
			this.ckBxLeftRight.Checked = true;
			this.ckBxLeftRight.CheckState = System.Windows.Forms.CheckState.Checked;
			this.ckBxLeftRight.Location = new System.Drawing.Point(681, 448);
			this.ckBxLeftRight.Name = "ckBxLeftRight";
			this.ckBxLeftRight.Size = new System.Drawing.Size(167, 24);
			this.ckBxLeftRight.TabIndex = 9;
			this.ckBxLeftRight.Text = "Has Left/Right Dimension";
			this.ckBxLeftRight.UseCompatibleTextRendering = true;
			this.ckBxLeftRight.UseVisualStyleBackColor = true;
			this.ckBxLeftRight.CheckedChanged += new System.EventHandler(this.CkBxUpCheckedChanged);
			// 
			// Dlg
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(856, 522);
			this.Controls.Add(this.ckBxLeft);
			this.Controls.Add(this.ckBxLeftRight);
			this.Controls.Add(this.ckBxTopBottom);
			this.Controls.Add(this.ckBxBottom);
			this.Controls.Add(this.btnEdgRight);
			this.Controls.Add(this.btnEdgDown);
			this.Controls.Add(this.btnEdgLeft);
			this.Controls.Add(this.btnEdgUp);
			this.Controls.Add(this.chBxV);
			this.Controls.Add(this.chBxH);
			this.Controls.Add(this.listBx);
			this.Controls.Add(this.lb);
			this.Controls.Add(this.btnPrev);
			this.Controls.Add(this.btnCreate);
			this.Controls.Add(this.btnCancel);
			this.Controls.Add(this.btnNext);
			this.Controls.Add(this.picBx);
			this.Margin = new System.Windows.Forms.Padding(2);
			this.Name = "Dlg";
			this.Text = "Dlg";
			this.Load += new System.EventHandler(this.DlgLoad);
			((System.ComponentModel.ISupportInitialize)(this.picBx)).EndInit();
			this.ResumeLayout(false);
		}
		private System.Windows.Forms.CheckBox ckBxLeftRight;
		private System.Windows.Forms.CheckBox ckBxTopBottom;
		private System.Windows.Forms.CheckBox ckBxLeft;
		private System.Windows.Forms.CheckBox ckBxBottom;
		private System.Windows.Forms.Button btnEdgRight;
		private System.Windows.Forms.Button btnEdgDown;
		private System.Windows.Forms.Button btnEdgLeft;
		private System.Windows.Forms.Button btnEdgUp;
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
