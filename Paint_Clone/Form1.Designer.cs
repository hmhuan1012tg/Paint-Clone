namespace Paint_Clone
{
    partial class Form1
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.lineRadio = new System.Windows.Forms.RadioButton();
            this.selectRadio = new System.Windows.Forms.RadioButton();
            this.rectRadio = new System.Windows.Forms.RadioButton();
            this.parallelogramRadio = new System.Windows.Forms.RadioButton();
            this.polygonRadio = new System.Windows.Forms.RadioButton();
            this.brokenLineRadio = new System.Windows.Forms.RadioButton();
            this.circleArcRadio = new System.Windows.Forms.RadioButton();
            this.circleRadio = new System.Windows.Forms.RadioButton();
            this.ellipseRadio = new System.Windows.Forms.RadioButton();
            this.ellipseArcRadio = new System.Windows.Forms.RadioButton();
            this.bezierRadio = new System.Windows.Forms.RadioButton();
            this.clearBtn = new System.Windows.Forms.Button();
            this.newTextRadio = new System.Windows.Forms.RadioButton();
            this.moveRadio = new System.Windows.Forms.RadioButton();
            this.deleteRadio = new System.Windows.Forms.RadioButton();
            this.controlRadio = new System.Windows.Forms.RadioButton();
            this.scaleRadio = new System.Windows.Forms.RadioButton();
            this.rotateRadio = new System.Windows.Forms.RadioButton();
            this.parabolaRadio = new System.Windows.Forms.RadioButton();
            this.hyperbolaRadio = new System.Windows.Forms.RadioButton();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.SuspendLayout();
            // 
            // pictureBox1
            // 
            this.pictureBox1.BackColor = System.Drawing.Color.White;
            this.pictureBox1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pictureBox1.Location = new System.Drawing.Point(12, 85);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(1160, 564);
            this.pictureBox1.TabIndex = 0;
            this.pictureBox1.TabStop = false;
            this.pictureBox1.Paint += new System.Windows.Forms.PaintEventHandler(this.pictureBox1_Paint);
            this.pictureBox1.MouseDown += new System.Windows.Forms.MouseEventHandler(this.pictureBox1_MouseDown);
            this.pictureBox1.MouseEnter += new System.EventHandler(this.pictureBox1_MouseEnter);
            this.pictureBox1.MouseLeave += new System.EventHandler(this.pictureBox1_MouseLeave);
            this.pictureBox1.MouseMove += new System.Windows.Forms.MouseEventHandler(this.pictureBox1_MouseMove);
            // 
            // lineRadio
            // 
            this.lineRadio.AutoSize = true;
            this.lineRadio.Location = new System.Drawing.Point(12, 12);
            this.lineRadio.Name = "lineRadio";
            this.lineRadio.Size = new System.Drawing.Size(45, 17);
            this.lineRadio.TabIndex = 1;
            this.lineRadio.TabStop = true;
            this.lineRadio.Text = "Line";
            this.lineRadio.UseVisualStyleBackColor = true;
            this.lineRadio.CheckedChanged += new System.EventHandler(this.radio_CheckedChanged);
            // 
            // selectRadio
            // 
            this.selectRadio.AutoSize = true;
            this.selectRadio.Location = new System.Drawing.Point(857, 12);
            this.selectRadio.Name = "selectRadio";
            this.selectRadio.Size = new System.Drawing.Size(55, 17);
            this.selectRadio.TabIndex = 2;
            this.selectRadio.TabStop = true;
            this.selectRadio.Text = "Select";
            this.selectRadio.UseVisualStyleBackColor = true;
            this.selectRadio.CheckedChanged += new System.EventHandler(this.radio_CheckedChanged);
            // 
            // rectRadio
            // 
            this.rectRadio.AutoSize = true;
            this.rectRadio.Location = new System.Drawing.Point(100, 12);
            this.rectRadio.Name = "rectRadio";
            this.rectRadio.Size = new System.Drawing.Size(74, 17);
            this.rectRadio.TabIndex = 3;
            this.rectRadio.TabStop = true;
            this.rectRadio.Text = "Rectangle";
            this.rectRadio.UseVisualStyleBackColor = true;
            this.rectRadio.CheckedChanged += new System.EventHandler(this.radio_CheckedChanged);
            // 
            // parallelogramRadio
            // 
            this.parallelogramRadio.AutoSize = true;
            this.parallelogramRadio.Location = new System.Drawing.Point(100, 35);
            this.parallelogramRadio.Name = "parallelogramRadio";
            this.parallelogramRadio.Size = new System.Drawing.Size(88, 17);
            this.parallelogramRadio.TabIndex = 4;
            this.parallelogramRadio.TabStop = true;
            this.parallelogramRadio.Text = "Parallelogram";
            this.parallelogramRadio.UseVisualStyleBackColor = true;
            this.parallelogramRadio.CheckedChanged += new System.EventHandler(this.radio_CheckedChanged);
            // 
            // polygonRadio
            // 
            this.polygonRadio.AutoSize = true;
            this.polygonRadio.Location = new System.Drawing.Point(180, 12);
            this.polygonRadio.Name = "polygonRadio";
            this.polygonRadio.Size = new System.Drawing.Size(63, 17);
            this.polygonRadio.TabIndex = 5;
            this.polygonRadio.TabStop = true;
            this.polygonRadio.Text = "Polygon";
            this.polygonRadio.UseVisualStyleBackColor = true;
            this.polygonRadio.CheckedChanged += new System.EventHandler(this.radio_CheckedChanged);
            // 
            // brokenLineRadio
            // 
            this.brokenLineRadio.AutoSize = true;
            this.brokenLineRadio.Location = new System.Drawing.Point(12, 35);
            this.brokenLineRadio.Name = "brokenLineRadio";
            this.brokenLineRadio.Size = new System.Drawing.Size(82, 17);
            this.brokenLineRadio.TabIndex = 6;
            this.brokenLineRadio.TabStop = true;
            this.brokenLineRadio.Text = "Broken Line";
            this.brokenLineRadio.UseVisualStyleBackColor = true;
            this.brokenLineRadio.CheckedChanged += new System.EventHandler(this.radio_CheckedChanged);
            // 
            // circleArcRadio
            // 
            this.circleArcRadio.AutoSize = true;
            this.circleArcRadio.Location = new System.Drawing.Point(310, 12);
            this.circleArcRadio.Name = "circleArcRadio";
            this.circleArcRadio.Size = new System.Drawing.Size(70, 17);
            this.circleArcRadio.TabIndex = 7;
            this.circleArcRadio.TabStop = true;
            this.circleArcRadio.Text = "Circle Arc";
            this.circleArcRadio.UseVisualStyleBackColor = true;
            this.circleArcRadio.CheckedChanged += new System.EventHandler(this.radio_CheckedChanged);
            // 
            // circleRadio
            // 
            this.circleRadio.AutoSize = true;
            this.circleRadio.Location = new System.Drawing.Point(249, 12);
            this.circleRadio.Name = "circleRadio";
            this.circleRadio.Size = new System.Drawing.Size(51, 17);
            this.circleRadio.TabIndex = 8;
            this.circleRadio.TabStop = true;
            this.circleRadio.Text = "Circle";
            this.circleRadio.UseVisualStyleBackColor = true;
            this.circleRadio.CheckedChanged += new System.EventHandler(this.radio_CheckedChanged);
            // 
            // ellipseRadio
            // 
            this.ellipseRadio.AutoSize = true;
            this.ellipseRadio.Location = new System.Drawing.Point(249, 35);
            this.ellipseRadio.Name = "ellipseRadio";
            this.ellipseRadio.Size = new System.Drawing.Size(55, 17);
            this.ellipseRadio.TabIndex = 9;
            this.ellipseRadio.TabStop = true;
            this.ellipseRadio.Text = "Ellipse";
            this.ellipseRadio.UseVisualStyleBackColor = true;
            this.ellipseRadio.CheckedChanged += new System.EventHandler(this.radio_CheckedChanged);
            // 
            // ellipseArcRadio
            // 
            this.ellipseArcRadio.AutoSize = true;
            this.ellipseArcRadio.Location = new System.Drawing.Point(310, 35);
            this.ellipseArcRadio.Name = "ellipseArcRadio";
            this.ellipseArcRadio.Size = new System.Drawing.Size(74, 17);
            this.ellipseArcRadio.TabIndex = 10;
            this.ellipseArcRadio.TabStop = true;
            this.ellipseArcRadio.Text = "Ellipse Arc";
            this.ellipseArcRadio.UseVisualStyleBackColor = true;
            this.ellipseArcRadio.CheckedChanged += new System.EventHandler(this.radio_CheckedChanged);
            // 
            // bezierRadio
            // 
            this.bezierRadio.AutoSize = true;
            this.bezierRadio.Location = new System.Drawing.Point(390, 12);
            this.bezierRadio.Name = "bezierRadio";
            this.bezierRadio.Size = new System.Drawing.Size(54, 17);
            this.bezierRadio.TabIndex = 11;
            this.bezierRadio.TabStop = true;
            this.bezierRadio.Text = "Bezier";
            this.bezierRadio.UseVisualStyleBackColor = true;
            this.bezierRadio.CheckedChanged += new System.EventHandler(this.radio_CheckedChanged);
            // 
            // clearBtn
            // 
            this.clearBtn.Location = new System.Drawing.Point(1097, 9);
            this.clearBtn.Name = "clearBtn";
            this.clearBtn.Size = new System.Drawing.Size(75, 23);
            this.clearBtn.TabIndex = 12;
            this.clearBtn.Text = "Clear";
            this.clearBtn.UseVisualStyleBackColor = true;
            this.clearBtn.Click += new System.EventHandler(this.clearBtn_Click);
            // 
            // newTextRadio
            // 
            this.newTextRadio.AutoSize = true;
            this.newTextRadio.Location = new System.Drawing.Point(523, 12);
            this.newTextRadio.Name = "newTextRadio";
            this.newTextRadio.Size = new System.Drawing.Size(71, 17);
            this.newTextRadio.TabIndex = 13;
            this.newTextRadio.TabStop = true;
            this.newTextRadio.Text = "New Text";
            this.newTextRadio.UseVisualStyleBackColor = true;
            this.newTextRadio.CheckedChanged += new System.EventHandler(this.radio_CheckedChanged);
            // 
            // moveRadio
            // 
            this.moveRadio.AutoSize = true;
            this.moveRadio.Location = new System.Drawing.Point(857, 35);
            this.moveRadio.Name = "moveRadio";
            this.moveRadio.Size = new System.Drawing.Size(52, 17);
            this.moveRadio.TabIndex = 14;
            this.moveRadio.TabStop = true;
            this.moveRadio.Text = "Move";
            this.moveRadio.UseVisualStyleBackColor = true;
            this.moveRadio.CheckedChanged += new System.EventHandler(this.radio_CheckedChanged);
            // 
            // deleteRadio
            // 
            this.deleteRadio.AutoSize = true;
            this.deleteRadio.Location = new System.Drawing.Point(979, 12);
            this.deleteRadio.Name = "deleteRadio";
            this.deleteRadio.Size = new System.Drawing.Size(56, 17);
            this.deleteRadio.TabIndex = 15;
            this.deleteRadio.TabStop = true;
            this.deleteRadio.Text = "Delete";
            this.deleteRadio.UseVisualStyleBackColor = true;
            this.deleteRadio.CheckedChanged += new System.EventHandler(this.radio_CheckedChanged);
            // 
            // controlRadio
            // 
            this.controlRadio.AutoSize = true;
            this.controlRadio.Location = new System.Drawing.Point(915, 12);
            this.controlRadio.Name = "controlRadio";
            this.controlRadio.Size = new System.Drawing.Size(58, 17);
            this.controlRadio.TabIndex = 16;
            this.controlRadio.TabStop = true;
            this.controlRadio.Text = "Control";
            this.controlRadio.UseVisualStyleBackColor = true;
            this.controlRadio.CheckedChanged += new System.EventHandler(this.radio_CheckedChanged);
            // 
            // scaleRadio
            // 
            this.scaleRadio.AutoSize = true;
            this.scaleRadio.Location = new System.Drawing.Point(915, 35);
            this.scaleRadio.Name = "scaleRadio";
            this.scaleRadio.Size = new System.Drawing.Size(52, 17);
            this.scaleRadio.TabIndex = 17;
            this.scaleRadio.TabStop = true;
            this.scaleRadio.Text = "Scale";
            this.scaleRadio.UseVisualStyleBackColor = true;
            this.scaleRadio.CheckedChanged += new System.EventHandler(this.radio_CheckedChanged);
            // 
            // rotateRadio
            // 
            this.rotateRadio.AutoSize = true;
            this.rotateRadio.Location = new System.Drawing.Point(979, 35);
            this.rotateRadio.Name = "rotateRadio";
            this.rotateRadio.Size = new System.Drawing.Size(57, 17);
            this.rotateRadio.TabIndex = 18;
            this.rotateRadio.TabStop = true;
            this.rotateRadio.Text = "Rotate";
            this.rotateRadio.UseVisualStyleBackColor = true;
            this.rotateRadio.CheckedChanged += new System.EventHandler(this.radio_CheckedChanged);
            // 
            // parabolaRadio
            // 
            this.parabolaRadio.AutoSize = true;
            this.parabolaRadio.Location = new System.Drawing.Point(450, 12);
            this.parabolaRadio.Name = "parabolaRadio";
            this.parabolaRadio.Size = new System.Drawing.Size(67, 17);
            this.parabolaRadio.TabIndex = 19;
            this.parabolaRadio.TabStop = true;
            this.parabolaRadio.Text = "Parabola";
            this.parabolaRadio.UseVisualStyleBackColor = true;
            this.parabolaRadio.CheckedChanged += new System.EventHandler(this.radio_CheckedChanged);
            // 
            // hyperbolaRadio
            // 
            this.hyperbolaRadio.AutoSize = true;
            this.hyperbolaRadio.Location = new System.Drawing.Point(450, 35);
            this.hyperbolaRadio.Name = "hyperbolaRadio";
            this.hyperbolaRadio.Size = new System.Drawing.Size(73, 17);
            this.hyperbolaRadio.TabIndex = 20;
            this.hyperbolaRadio.TabStop = true;
            this.hyperbolaRadio.Text = "Hyperbola";
            this.hyperbolaRadio.UseVisualStyleBackColor = true;
            this.hyperbolaRadio.CheckedChanged += new System.EventHandler(this.radio_CheckedChanged);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.ControlLight;
            this.ClientSize = new System.Drawing.Size(1184, 661);
            this.Controls.Add(this.hyperbolaRadio);
            this.Controls.Add(this.parabolaRadio);
            this.Controls.Add(this.rotateRadio);
            this.Controls.Add(this.scaleRadio);
            this.Controls.Add(this.controlRadio);
            this.Controls.Add(this.deleteRadio);
            this.Controls.Add(this.moveRadio);
            this.Controls.Add(this.newTextRadio);
            this.Controls.Add(this.clearBtn);
            this.Controls.Add(this.bezierRadio);
            this.Controls.Add(this.ellipseArcRadio);
            this.Controls.Add(this.ellipseRadio);
            this.Controls.Add(this.circleRadio);
            this.Controls.Add(this.circleArcRadio);
            this.Controls.Add(this.brokenLineRadio);
            this.Controls.Add(this.polygonRadio);
            this.Controls.Add(this.parallelogramRadio);
            this.Controls.Add(this.rectRadio);
            this.Controls.Add(this.selectRadio);
            this.Controls.Add(this.lineRadio);
            this.Controls.Add(this.pictureBox1);
            this.DoubleBuffered = true;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.KeyPreview = true;
            this.MaximizeBox = false;
            this.Name = "Form1";
            this.Text = "Form1";
            this.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.Form1_KeyPress);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.RadioButton lineRadio;
        private System.Windows.Forms.RadioButton selectRadio;
        private System.Windows.Forms.RadioButton rectRadio;
        private System.Windows.Forms.RadioButton parallelogramRadio;
        private System.Windows.Forms.RadioButton polygonRadio;
        private System.Windows.Forms.RadioButton brokenLineRadio;
        private System.Windows.Forms.RadioButton circleArcRadio;
        private System.Windows.Forms.RadioButton circleRadio;
        private System.Windows.Forms.RadioButton ellipseRadio;
        private System.Windows.Forms.RadioButton ellipseArcRadio;
        private System.Windows.Forms.RadioButton bezierRadio;
        private System.Windows.Forms.Button clearBtn;
        private System.Windows.Forms.RadioButton newTextRadio;
        private System.Windows.Forms.RadioButton moveRadio;
        private System.Windows.Forms.RadioButton deleteRadio;
        private System.Windows.Forms.RadioButton controlRadio;
        private System.Windows.Forms.RadioButton scaleRadio;
        private System.Windows.Forms.RadioButton rotateRadio;
        private System.Windows.Forms.RadioButton parabolaRadio;
        private System.Windows.Forms.RadioButton hyperbolaRadio;
    }
}

