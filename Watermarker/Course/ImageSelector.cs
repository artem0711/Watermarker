using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Course
{
    public partial class ImageSelector : UserControl // Контрол, который отображает картинку на форме
    {
        public ImageSelector()
        {
            InitializeComponent(); //Инициализируем его
        }
        public Image Image //Свойство, отвечающее за картинку. Берем/кидаем картинку из пикчербокса/в пикчербокс
        {
            get { return pictureBox1.Image; } 
            set { pictureBox1.Image = value; } 
        }
        public PictureBoxSizeMode SizeMode //Свойство, отвечающее за тип отображения картинки. Берем/кидаем тип отображения из пикчербокса/в пикчербокс
        {
            get { return pictureBox1.SizeMode; }
            set { pictureBox1.SizeMode = value; }
        }
        public bool Checked //Свойство, отвечающее за отметку картинки галочкой. Берем/кидаем отметку из чекбокса/в чекбокс
        {
            get { return checkBox1.Checked; }
            set { checkBox1.Checked = value; }

        }
        public override string Text//Свойство, отвечающее за подпись под картинкой. Берем/кидаем подпись из чекбокса/в чекбокс
        {
            get { return checkBox1.Text; }
            set { checkBox1.Text = value; }
        }
        
        public void pictureBox1_Click(object sender, EventArgs e) //Событие клика по картинке
        {
            this.Checked = !this.Checked; //Меняем положение галочки под картинкой (Ставим или убираем).
        }
    }
}
