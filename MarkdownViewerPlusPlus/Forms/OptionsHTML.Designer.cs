namespace com.insanitydesign.MarkdownViewerPlusPlus.Forms
{
    partial class OptionsHTML
    {
        /// <summary>
        /// 
        /// </summary>
        private System.Windows.Forms.GroupBox grpStyles;

        /// <summary>
        /// 
        /// </summary>
        protected override void InitializeComponent()
        {
            this.grpStyles = new System.Windows.Forms.GroupBox();
            this.txtCssStyles = new System.Windows.Forms.TextBox();
            this.lblCssStyles = new System.Windows.Forms.Label();
            this.grpStyles.SuspendLayout();
            this.SuspendLayout();
            // 
            // grpStyles
            // 
            this.grpStyles.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.grpStyles.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.grpStyles.Controls.Add(this.lblCssStyles);
            this.grpStyles.Controls.Add(this.txtCssStyles);
            this.grpStyles.Location = new System.Drawing.Point(4, 4);
            this.grpStyles.Name = "grpStyles";
            this.grpStyles.Size = new System.Drawing.Size(1248, 169);
            this.grpStyles.TabIndex = 0;
            this.grpStyles.TabStop = false;
            this.grpStyles.Text = "Styles";
            // 
            // txtCssStyles
            // 
            this.txtCssStyles.AcceptsReturn = true;
            this.txtCssStyles.AcceptsTab = true;
            this.txtCssStyles.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtCssStyles.Location = new System.Drawing.Point(10, 39);
            this.txtCssStyles.Multiline = true;
            this.txtCssStyles.Name = "txtCssStyles";
            this.txtCssStyles.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.txtCssStyles.Size = new System.Drawing.Size(1232, 124);
            this.txtCssStyles.TabIndex = 0;
            // 
            // lblCssStyles
            // 
            this.lblCssStyles.AutoSize = true;
            this.lblCssStyles.Location = new System.Drawing.Point(7, 20);
            this.lblCssStyles.Name = "lblCssStyles";
            this.lblCssStyles.Size = new System.Drawing.Size(66, 13);
            this.lblCssStyles.TabIndex = 1;
            this.lblCssStyles.Text = "Custom CSS";
            // 
            // OptionsHTML
            // 
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.Controls.Add(this.grpStyles);
            this.Name = "OptionsHTML";
            this.Size = new System.Drawing.Size(1255, 709);
            this.grpStyles.ResumeLayout(false);
            this.grpStyles.PerformLayout();
            this.ResumeLayout(false);

        }

        private System.Windows.Forms.Label lblCssStyles;
        private System.Windows.Forms.TextBox txtCssStyles;
    }
}
