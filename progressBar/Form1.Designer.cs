namespace progressBar
{
    partial class Form1
    {
        /// <summary>
        /// Wymagana zmienna projektanta.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Wyczyść wszystkie używane zasoby.
        /// </summary>
        /// <param name="disposing">prawda, jeżeli zarządzane zasoby powinny zostać zlikwidowane; Fałsz w przeciwnym wypadku.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Kod generowany przez Projektanta formularzy systemu Windows

        /// <summary>
        /// Metoda wymagana do obsługi projektanta — nie należy modyfikować
        /// jej zawartości w edytorze kodu.
        /// </summary>
        private void InitializeComponent()
        {
            this.cRoundedProgressBar1 = new progressBar.cRoundedProgressBar();
            this.SuspendLayout();
            // 
            // cRoundedProgressBar1
            // 
            this.cRoundedProgressBar1.BarColor = System.Drawing.Color.SteelBlue;
            this.cRoundedProgressBar1.Location = new System.Drawing.Point(557, 24);
            this.cRoundedProgressBar1.Name = "cRoundedProgressBar1";
            this.cRoundedProgressBar1.Percent = 97;
            this.cRoundedProgressBar1.Size = new System.Drawing.Size(440, 350);
            this.cRoundedProgressBar1.TabIndex = 0;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1009, 450);
            this.Controls.Add(this.cRoundedProgressBar1);
            this.Name = "Form1";
            this.Text = "Form1";
            this.ResumeLayout(false);

        }

        #endregion

        private cRoundedProgressBar cRoundedProgressBar1;
    }
}

