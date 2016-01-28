using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.Threading;
using System.Reflection;

namespace Course
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent(); //Инициализация главного окна.
        }

        int diff = 5; //расстояние между картинками на форме (в пикселах)

        class ImageStruct //класс для загрузки картинок
        {
            public Image Image; //загружаемая картинка
            public string Path; //путь, по которому загружается картинка
            public int Index; //номер этого элемента в массиве картинок
            public ImageSelector Container; //контейнер для картинки (чтобы мы могли её нарисовать на форме)
        }
        ImageStruct[] Images = null; //массив классов, которые будут хранить наши картинки


        void ClearImages() //очистить массив
        {
            Images = null;
        }

        readonly string[] imageFormats = { "png", "jpg", "gif" }; //допустимые форматы картинок
        Thread[] loadThreads; //нити, которые будут загружать картинки в программу

        private void button1_Click(object sender, EventArgs e) //событие нажатия на кнопку "Открыть папку"
        {


            FolderBrowserDialog fbd = new FolderBrowserDialog(); //диалог открытия папки
            if (fbd.ShowDialog() == DialogResult.OK) //если, когда мы его вызвали, пользователь нажал "ОК" (а не, например, "Отмена"), то
            {

                List<string> imageFiles = new List<string>(); //создаем новый список, в котором будем хранить пути к нашим изображениям

                foreach (var extension in imageFormats) //для каждого расширения в массиве расширений:
                {
                    imageFiles.AddRange(Directory.GetFiles(fbd.SelectedPath, "*." + extension).ToList()); //ищем все файлы в указанной папке, у которых такое расширение и добавляем их в список
                }


                //Теперь мы знаем, сколько у нас будет картинок. Переопределяем массивы, чтобы элементов в них было столько же:
                Images = new ImageStruct[imageFiles.Count];
                loadThreads = new Thread[imageFiles.Count];


                //Теперь мы будем загружать элементы в массив

                for (int i = 0; i < Images.Length; i++) //Проходим по всему массиву
                {
                    Images[i] = new ImageStruct //Загружаем в массив путь к файлу и его номер
                    {
                        Index = i,
                        Path = imageFiles[i]
                    };
                    //Саму картинку загрузим через нить. Для этого:
                    loadThreads[i] = new Thread(LoadImage); //Инициализируем нить
                    loadThreads[i].Start(Images[i]); //Запускаем её, передав наши аргументы (в том числе, адрес картинки)
                }


                foreach (var t in loadThreads) //Ждём, пока все нити закончат работать
                {
                    t.Join();
                }
                //На этом этапе нити закончили работать, а значит, загрузили наши картинки. Теперь добавим эти картинки на форму.

                panel1.Controls.Clear(); //Очищаем панель, чтобы загрузить в неё новые картинки

                foreach (ImageStruct myImageStruct in Images) //проходим по всем элементам в нашем массиве
                {
                    panel1.Controls.Add(myImageStruct.Container); //Добавляем картинку из каждого элемента на форму.
                }
            }
        }

        void LoadImage(object args) //Процедура загрузки картинок (вызывается из нитей выше)
        {
            int numberInaRow = getNumberInaRow(); //Вычисляем, сколько картинок поместится в одной линии
            ImageStruct myImageStruct = (ImageStruct)args; //Преобразуем аргумент этой функции в нужный нам тип
            myImageStruct.Container = new ImageSelector(); //Создадим новый контейнер картинки
            myImageStruct.Container.SizeMode = templateImageSelector.SizeMode; //Скопируем из шаблона его тип отображения картинки (чтобы она полностью поместилась в маленьком квадратике)
            myImageStruct.Container.Size = templateImageSelector.Size; //Скопируем из шаблона его размер
            myImageStruct.Container.Top = templateImageSelector.Top + (myImageStruct.Index / numberInaRow) * (templateImageSelector.Height + 5); //Вычислим, где должна находиться картинка на панели по оси Y
            myImageStruct.Container.Left = (myImageStruct.Index % (numberInaRow)) * (templateImageSelector.Width + 5);//Вычислим, где должна находиться картинка на панели по оси Y
            myImageStruct.Container.BackColor = templateImageSelector.BackColor; //Скопируем из шаблона цвет фона
            myImageStruct.Image = Image.FromFile(myImageStruct.Path); //Загрузим из файла саму картинку
            myImageStruct.Container.Visible = true; //Сделаем её видимой на форме
            myImageStruct.Container.Image = myImageStruct.Image; //Нарисуем картинку на форме
            myImageStruct.Container.Text = getFileName(myImageStruct.Path); //Нарисуем в подписи к картинке её название

        }

        string fileFormat(string path) //Определяем формат файла по его пути
        {
            string[] sep = path.Split('.'); //Создаем массив и разбиваем исходный файл в этот массив по символу "." 
            return sep[sep.Length - 1];  //Тогда последний элемент в этом массиве - и есть формат файла. Возвращаем его.
        }
        string getFileName(string path) //Определяем имя файла по его пути
        {
            string[] sep = path.Split('\\'); //Разбиваем путь к файлу по разделителю и засовываем в массив.
            return sep[sep.Length - 1]; //Тогда последний элемент в этом массиве - и есть имя файла. Возвращаем его.
        }


        private void button2_Click(object sender, EventArgs e) //При нажатии на кнопка "Выделить всё"
        {
            foreach (ImageSelector imSel in panel1.Controls) //Проходим по всем элементам в панели
            {
                imSel.Checked = true; //И отмечаем их галочкой
            }
        }

        private void button3_Click(object sender, EventArgs e) //При нажатии на кнопка "Выделить ничего"
        {
            foreach (ImageSelector imSel in panel1.Controls) //Проходим по всем элементам в панели
            {
                imSel.Checked = false; //И снимаем с них галочки
            }
        }

        private int getNumberInaRow() //Определяем, сколько картинок поместится в одной строчке
        {
            int imageW = templateImageSelector.Width; //Ширина картинки
            int panelW = panel1.Width - 10; //Ширина строчки (-10, чтобы поместилась полоска прокрутки)
            return Math.Max(1, (int)(panelW / (imageW + diff))); //Делим ширину полоски на ширину картинки и получаем количество картинок. Если количество вышло меньше 1, то возвращаем 1.
        }

        private void Form1_ResizeEnd(object sender, EventArgs e) //Если пользователь меняет размер формы
        {
            setImageLocations(); //То мы меняем расположение картинок на панели

        }

        FormWindowState LastWindowState = FormWindowState.Normal; //Запоминаем предыдущее состояние формы (форма может быть свернутая, развернутая, обычная)

        private void Form1_Resize(object sender, EventArgs e)
        {
            if (this.WindowState != LastWindowState) //Если состояние формы поменялось
            {
                setImageLocations(); //То мы меняем расположение картинок на панели
                LastWindowState = this.WindowState; //И снова запоминаем предыдущее состояние формы
            }

        }

        void setImageLocations() //Изменение расположения картинок на панели
        {
            if (Images != null) //Если картинки вообще есть, то
            {

                int numberInaRow = getNumberInaRow(); //Вычисляем, сколько картинок будет в каждом ряду
                foreach (ImageStruct ims in Images) //Проходим по всем картинкам
                {
                    ims.Container.Top = templateImageSelector.Top + (ims.Index / numberInaRow) * (templateImageSelector.Height + diff); //Меняем их расположение по оси Y
                    ims.Container.Left = (ims.Index % (numberInaRow)) * (templateImageSelector.Width + diff); //Меняем их расположение по оси X
                }
            }
        }

        List<Thread> saveThreads; //Список нитей, которые будут сохранять картинки

        class SaveStruct //Класс, в который мы положим картинки, чтобы их сохранить
        {
            public Image Image; //Сама картинка
            public string Path; //Путь, по которому мы её сохраняем
            public Font Font; //Шрифт, которым мы наложим водяной знак
            public SaveStruct(Image image, string path, Font font) //Конструктор этого класса
            {
                this.Image = image;
                this.Path = path;
                this.Font = font;
            }
        }

        private void button4_Click(object sender, EventArgs e) //Событие нажатия кнопки "Сохранить картинку"
        {
            if (Images != null) //Если картинки вообще есть, то
            {
                FolderBrowserDialog ofd = new FolderBrowserDialog(); //Инициализируем диалог выбора папки для сохранения

                FontDialog fd = new FontDialog(); //Инициализируем диалог выбора шрифта водяного знака

                if (fd.ShowDialog() == DialogResult.OK) //Если в диалоге выбора шрифта мы нажали ОК
                {
                    if (ofd.ShowDialog() == DialogResult.OK) //И в диалоге выбора папки мы нажали ОК, то
                    {

                        saveThreads = new List<Thread>(); //Инициализируем список нитей для сохранения
                        for (int i = 0; i < Images.Length; i++) //Проходим по всем загруженным картинкам
                        {
                            if (Images[i].Container.Checked) //Если картинка отмечена галочкой (т.е. её надо сохранить)
                            {

                                string savePath = ofd.SelectedPath + '\\' + getFileName(Images[i].Path); //Вычисляем путь, по которому будем сохранять эту картинку
                                SaveStruct saver = new SaveStruct(Images[i].Image, savePath, fd.Font); //Создаем экземпляр класса для сохранения
                                saveThreads.Add(new Thread(EditAndSave)); //Создаем новую нить для сохранения картинки и добавляем её в список нитей
                                saveThreads[saveThreads.Count - 1].Start(saver);//Запускаем нашу нить. Она будет последним элементом в списке (потому что мы её только что туда добавили). 
                            }
                        }

                        foreach (var t in saveThreads) //Ждём, пока все нити завершат работу
                        {
                            t.Join();
                        }

                        MessageBox.Show("Completed!"); //Выводим сообщение о том, что работа завершена
                    }

                }

            }

        }
        void EditAndSave(object args) //Метод наложения на картинку водяного знака и сохранения
        {
            SaveStruct saver = (SaveStruct)args; //Наш класс сохранения, который мы передали в аргументы
            using (Graphics g = Graphics.FromImage(saver.Image)) //Изпользуем графическую оболочку, чтобы нарисовать водяной знак. Загружаем в неё нашу картинку, поверх которой будем рисовать.
            {
                SizeF textSize = g.MeasureString(waterMarkTextBox.Text, saver.Font); //Считаем, какая высота и ширина будет у нашего водяного знака
                SolidBrush semiTransBrush = new SolidBrush(Color.FromArgb(128, 0, 0, 0)); //Инициализируем кисть, которой будет рисоваться водяной знак. Кисть делаем полупрозрачной.
                /* for (float w = 0; w < saver.Image.Width; w += textSize.Width) //Проходимся по ширине картинки с шагом в ширину водяного знака
                {
                    for (float h = 0; h < saver.Image.Height; h += textSize.Height) //Проходимся по высоте картинки с шагом в высоту водяного знака
                    {
                        g.DrawString(waterMarkTextBox.Text, saver.Font, semiTransBrush, new PointF(w, h)); //Рисуем поверх картинки водяной знак
                    }
                }
                 */

                float w = saver.Image.Width / 2 - textSize.Width / 2;
                float h = saver.Image.Height - textSize.Height - 5;
                g.DrawString(waterMarkTextBox.Text, saver.Font, semiTransBrush, new PointF(w, h));

                try //Пробуем сохранить картинку
                { 

                    saver.Image.Save(saver.Path);
                }
                catch (Exception ex) //Если сохранить не вышло, выводим сообщение об ошибке
                {
                    MessageBox.Show(ex.Message);
                }
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

    }
}
