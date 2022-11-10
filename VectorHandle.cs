using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KmeanPic
{
    class VectorHandle
    {
        //Read file 
        public static void input(string fileName, List<Picture> listItem)
        {
            listItem.Clear();
            var strs = File.ReadAllLines(fileName);
            foreach (var str in strs)
            {
                Picture img = new Picture();
                Vector vt = new Vector();
                //Split line 
                var vector = str.Trim().Split(' ', '(', ')');
                img.Id = vector[0];
                //Save feature vector
                for (int i = 1; i < vector.Length - 2; i++)
                {
                    if (vector[i].Contains(".jpg"))
                        img.ImgName = vector[i];
                    else if (Double.TryParse(vector[i], out double v))
                        vt.Properties.Add(v);
                }
                img.Vector = vt;
                //Save Folder Name
                img.FolderName = vector[vector.Length - 2];
                listItem.Add(img);
            }
        }
        //print file
        public static void output(List<Cluster> ClusterList)
        {
            foreach (var cluster in ClusterList)
            {
                cluster.PicturesListOC = cluster.PicturesListOC.OrderBy(clus => clus.Distance).ToList();
                cluster.Name = cluster.PicturesListOC[0].FolderName;
            }
            //sắp xếp theo độ ưu tiên nào nhỏ nhất thì càng gần tâm ( chính xác càng cao)
            foreach (var cluster in ClusterList)
            {
                string pictureString = String.Empty;
                foreach (var picture in cluster.PicturesListOC)
                {
                    pictureString += picture.Id + " (";
                    foreach (var property in picture.Vector.Properties)
                    {
                        pictureString += property + " ";
                    }
                    pictureString = pictureString.Substring(0, pictureString.Length - 1);
                    pictureString += ") (" + picture.ImgName + ") (" + picture.FolderName + ")\n";
                }
                pictureString = pictureString.Substring(0, pictureString.Length - 1);
                if(!File.Exists("../../Resources/Cluster/" + cluster.Name + ".txt"))
                    File.WriteAllText("../../Resources/Cluster/" + cluster.Name + ".txt", pictureString);
                else using (StreamWriter sw = File.AppendText("../../Resources/Cluster/" + cluster.Name + ".txt"))
                    {
                        sw.Write("\n"+ pictureString);
                    }
            }
        }
        //Distance function
        public static double Distance(Vector a, Vector b)
        {
            double sum = 0;
            for (int i = 0; i < a.Properties.Count; i++)
                sum += Math.Pow((a.Properties[i] - b.Properties[i]),2.0);
            return Math.Sqrt(sum);
        }
    }
}
