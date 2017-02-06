namespace com.insanitydesign.MarkdownViewerPlusPlus.Forms
{
    partial class MarkdownViewerOptions
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

            //
            if(this.optionsGeneral != null)
            {
                this.optionsGeneral.Dispose();
                this.optionsGeneral = null;
            }

            //
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.Windows.Forms.TreeNode treeNode1 = new System.Windows.Forms.TreeNode("General");
            System.Windows.Forms.TreeNode treeNode2 = new System.Windows.Forms.TreeNode("HTML", 1, 1);
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MarkdownViewerOptions));
            this.btnOptionsSave = new System.Windows.Forms.Button();
            this.btnOptionsCancel = new System.Windows.Forms.Button();
            this.splitOptions = new System.Windows.Forms.SplitContainer();
            this.treeOptions = new System.Windows.Forms.TreeView();
            this.imgOptions = new System.Windows.Forms.ImageList(this.components);
            ((System.ComponentModel.ISupportInitialize)(this.splitOptions)).BeginInit();
            this.splitOptions.Panel1.SuspendLayout();
            this.splitOptions.SuspendLayout();
            this.SuspendLayout();
            // 
            // btnOptionsSave
            // 
            this.btnOptionsSave.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnOptionsSave.Location = new System.Drawing.Point(455, 306);
            this.btnOptionsSave.Name = "btnOptionsSave";
            this.btnOptionsSave.Size = new System.Drawing.Size(75, 23);
            this.btnOptionsSave.TabIndex = 0;
            this.btnOptionsSave.Text = "Save";
            this.btnOptionsSave.UseVisualStyleBackColor = true;
            this.btnOptionsSave.Click += new System.EventHandler(this.btnOptionsSave_Click);
            // 
            // btnOptionsCancel
            // 
            this.btnOptionsCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnOptionsCancel.CausesValidation = false;
            this.btnOptionsCancel.Location = new System.Drawing.Point(374, 306);
            this.btnOptionsCancel.Name = "btnOptionsCancel";
            this.btnOptionsCancel.Size = new System.Drawing.Size(75, 23);
            this.btnOptionsCancel.TabIndex = 1;
            this.btnOptionsCancel.Text = "Cancel";
            this.btnOptionsCancel.UseVisualStyleBackColor = true;
            this.btnOptionsCancel.Click += new System.EventHandler(this.btnOptionsCancel_Click);
            // 
            // splitOptions
            // 
            this.splitOptions.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.splitOptions.Location = new System.Drawing.Point(0, 0);
            this.splitOptions.Name = "splitOptions";
            // 
            // splitOptions.Panel1
            // 
            this.splitOptions.Panel1.Controls.Add(this.treeOptions);
            this.splitOptions.Size = new System.Drawing.Size(530, 300);
            this.splitOptions.SplitterDistance = 135;
            this.splitOptions.TabIndex = 2;
            // 
            // treeOptions
            // 
            this.treeOptions.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.treeOptions.FullRowSelect = true;
            this.treeOptions.HideSelection = false;
            this.treeOptions.ImageIndex = 0;
            this.treeOptions.ImageList = this.imgOptions;
            this.treeOptions.Location = new System.Drawing.Point(0, 0);
            this.treeOptions.Name = "treeOptions";
            treeNode1.Checked = true;
            treeNode1.Name = "nodeGeneral";
            treeNode1.Tag = "optionsGeneral";
            treeNode1.Text = "General";
            treeNode2.ImageIndex = 1;
            treeNode2.Name = "nodeHTML";
            treeNode2.SelectedImageIndex = 1;
            treeNode2.Tag = "optionsHTML";
            treeNode2.Text = "HTML";
            this.treeOptions.Nodes.AddRange(new System.Windows.Forms.TreeNode[] {
            treeNode1,
            treeNode2});
            this.treeOptions.SelectedImageIndex = 0;
            this.treeOptions.Size = new System.Drawing.Size(135, 300);
            this.treeOptions.TabIndex = 0;
            // 
            // imgOptions
            // 
            this.imgOptions.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imgOptions.ImageStream")));
            this.imgOptions.TransparentColor = System.Drawing.Color.Transparent;
            this.imgOptions.Images.SetKeyName(0, "markdown-16x16-solid.png");
            this.imgOptions.Images.SetKeyName(1, "fa-html5-16x16.png");
            // 
            // MarkdownViewerOptions
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(534, 341);
            this.Controls.Add(this.splitOptions);
            this.Controls.Add(this.btnOptionsCancel);
            this.Controls.Add(this.btnOptionsSave);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.MinimumSize = new System.Drawing.Size(550, 380);
            this.Name = "MarkdownViewerOptions";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Options - MarkdownViewer++";
            this.splitOptions.Panel1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitOptions)).EndInit();
            this.splitOptions.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button btnOptionsSave;
        private System.Windows.Forms.Button btnOptionsCancel;
        private System.Windows.Forms.SplitContainer splitOptions;
        private System.Windows.Forms.TreeView treeOptions;
        private System.Windows.Forms.ImageList imgOptions;
    }
}