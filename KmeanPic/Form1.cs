using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace KmeanPic
{
    public partial class Form1 : DevExpress.XtraEditors.XtraForm
    {
        //Image list 
        List<Picture> PictureList = new List<Picture>();
        List<Cluster> ClusterList = new List<Cluster>();
        string fileName = Path.Combine(Environment.CurrentDirectory, @"..\..\Resources\", "FeatureVectors.txt");
        string folder = Path.Combine(Environment.CurrentDirectory, @"..\..\Resources\");

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            try
            {
                System.IO.DirectoryInfo di = new DirectoryInfo(folder + "/Cluster/");
                foreach (FileInfo file in di.GetFiles())
                {
                    file.Delete();
                }
                foreach (DirectoryInfo dir in di.GetDirectories())
                {
                    dir.Delete(true);
                }
                if (!Directory.EnumerateFileSystemEntries(folder + "/Cluster/").Any())
                {
                    VectorHandle.input(fileName, PictureList);
                    ClusterList = Kmean.kmean(PictureList);
                    VectorHandle.output(ClusterList);
                }    
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: Can't read file" + ex.Message, "Notify", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            //listView1.Items.Clear();
            //listView1.View = View.SmallIcon;
            ////tạo danh sách ảnh
            //ImageList listImg = new ImageList() { ImageSize = new Size(70, 70) };
            //listView1.SmallImageList = listImg;
            ////gán cho listView(lst)
            //int k = 0;
            //for (int i = 100; i < 110; i++)
            //{
            //    try
            //    {
            //        listImg.Images.Add(System.Drawing.Image.FromFile("../../Image/beach/" + i + ".jpg"));
            //    }
            //    catch (Exception ex)
            //    {
            //        continue;
            //    }
            //    ListViewItem item = new ListViewItem() { Text = i + ".jpg" };
            //    item.ImageIndex = k++;
            //    listView1.Items.Add(item);
            //}
        }

        private void simpleButton1_Click(object sender, EventArgs e)
        {
            if (xtraOpenFileDialog1.ShowDialog() == DialogResult.OK)
            {
                pictureEdit1.Image = System.Drawing.Image.FromFile(xtraOpenFileDialog1.FileName);
                textEdit1.Text = xtraOpenFileDialog1.SafeFileName;
                btnSearch.Enabled = true;
            }
        }

        private void simpleButton2_Click(object sender, EventArgs e)
        {
            //try
            //{
                search(xtraOpenFileDialog1.SafeFileName, listView1);

            //}
            //catch (Exception ex)
            //{
            //    MessageBox.Show("Error:" + ex.Message, "Notify", MessageBoxButtons.OK, MessageBoxIcon.Error);

            //}
        }
        public void search(string fileName, ListView lst)
        {
            //tìm ảnh có fileName truyền vào từ danh sách listItem
            Picture pic = PictureList.Find(picture => picture.ImgName.Equals(fileName));
            //tính khoảng cách từ ảnh đó đến từng cụm coi gần cụm nào nhất (khoảng cách min)
            double minDistance = double.MaxValue;
            string searchName = "";
            foreach (var cluster in ClusterList)
            {
                double distance = VectorHandle.Distance(cluster.VectorOC, pic.Vector);
                if (minDistance > distance)
                {
                    minDistance = distance;
                    searchName = cluster.Name;
                }
            }
            //khai báo danh sách những ảnh tương tự
            List<Picture> listSearch = new List<Picture>();
            //vào file có tên listCum[vt].TenThuMuc đọc ra
            var pictures = File.ReadAllLines(folder + "/Cluster/" + searchName + ".txt");
            //phần này giống như hàm đọc file trên (input())
            foreach (var picture in pictures)
            {
                pic = new Picture();
                Vector vector = new Vector();
                var str = picture.Trim().Split(' ', '(', ')');
                pic.Id = str[0];
                for (int i = 1; i < str.Length - 2; i++)
                {
                    if (str[i].Contains(".jpg"))
                        pic.ImgName = str[i];
                    else
                        try
                        {
                            vector.Properties.Add(Double.Parse(str[i]));
                        }
                        catch (Exception ex)
                        {
                            continue;
                        }
                }
                pic.Vector = vector;
                pic.FolderName = str[str.Length - 2];
                listSearch.Add(pic);
            }
            lst.Items.Clear();
            lst.View = View.SmallIcon;
            //tạo danh sách ảnh
            ImageList listImg = new ImageList() { ImageSize = new Size(70, 70) };
            //gán cho listView(lst)
            lst.SmallImageList = listImg;
            int k = 0;
            foreach (Picture image in listSearch)
            {
                try
                {
                    listImg.Images.Add(Image.FromFile(folder + "/Image/" + image.FolderName + "/" + image.ImgName));
                }
                catch (Exception ex)
                {
                    continue;
                }
                ListViewItem item = new ListViewItem() { Text = image.ImgName };
                item.ImageIndex = k++;
                lst.Items.Add(item);
            }
        }
    }
}
