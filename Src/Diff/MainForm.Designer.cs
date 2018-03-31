namespace Diff
{
    partial class MainForm
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
            this.elementHost1 = new System.Windows.Forms.Integration.ElementHost();
            this.searchTextBox = new Diff.CueTextBox();
            this.expressionEditor1 = new Diff.Editor.ExpressionEditor();
            this.SuspendLayout();
            // 
            // elementHost1
            // 
            this.elementHost1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.elementHost1.Location = new System.Drawing.Point(392, 36);
            this.elementHost1.Name = "elementHost1";
            this.elementHost1.Size = new System.Drawing.Size(692, 520);
            this.elementHost1.TabIndex = 6;
            this.elementHost1.Text = "elementHost1";
            this.elementHost1.Child = null;
            // 
            // searchTextBox
            // 
            this.searchTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.searchTextBox.CueText = "Search";
            this.searchTextBox.Font = new System.Drawing.Font("Consolas", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.searchTextBox.Location = new System.Drawing.Point(909, 5);
            this.searchTextBox.Name = "searchTextBox";
            this.searchTextBox.Size = new System.Drawing.Size(170, 26);
            this.searchTextBox.TabIndex = 7;
            this.searchTextBox.TextChanged += new System.EventHandler(this.searchTextBox_TextChanged);
            // 
            // expressionEditor1
            // 
            this.expressionEditor1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.expressionEditor1.AutoScroll = true;
            this.expressionEditor1.AutoScrollMinSize = new System.Drawing.Size(25, 50);
            this.expressionEditor1.Location = new System.Drawing.Point(0, 56);
            this.expressionEditor1.Name = "expressionEditor1";
            this.expressionEditor1.SelectedText = "";
            this.expressionEditor1.Size = new System.Drawing.Size(392, 500);
            this.expressionEditor1.TabIndex = 0;
            this.expressionEditor1.TextChanged += new System.EventHandler(this.expressionEditor1_TextChanged);
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(1084, 556);
            this.Controls.Add(this.searchTextBox);
            this.Controls.Add(this.elementHost1);
            this.Controls.Add(this.expressionEditor1);
            this.MinimumSize = new System.Drawing.Size(500, 300);
            this.Name = "MainForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Diff";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private Editor.ExpressionEditor expressionEditor1;
        private System.Windows.Forms.Integration.ElementHost elementHost1;
        private CueTextBox searchTextBox;
    }
}

