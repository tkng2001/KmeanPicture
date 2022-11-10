using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KmeanPic
{
    class Kmean
    {
        //Cluster Quantity
        static int ClusterQuantity = 10;
        
        //Kmean Algorithm
        public static List<Cluster> kmean(List<Picture> PictureList)
        {
            List<Cluster> ClusterList = new List<Cluster>();
            List<Cluster> newClusterList = new List<Cluster>();
            //Choose the root vector for each cluster(1-10 in vector list)
            for (int cluster = 0; cluster < ClusterQuantity; cluster++)
            {
                Cluster clt = new Cluster(PictureList[cluster].Vector);
                ClusterList.Add(clt);
            }
            //Copy ClusterList to newClusterList
            for (int cluster = 0; cluster < ClusterQuantity; cluster++)
            {
                Cluster clt = new Cluster(PictureList[cluster].Vector);
                newClusterList.Add(clt);
            }
            while (true)
            {
                foreach (var Picture in PictureList)
                {
                    //Calculate the distance between Image's Vector and Cluster's Vector
                    List<double> AllDistance = new List<double>();
                    foreach (var item in ClusterList)
                    {
                        AllDistance.Add(VectorHandle.Distance(Picture.Vector, item.VectorOC));
                    }
                    //Clustering for Picture
                    for (int cluster = 0; cluster < newClusterList.Count; cluster++)
                    {
                        if (VectorHandle.Distance(Picture.Vector, ClusterList[cluster].VectorOC) == AllDistance.Min())
                        {
                            newClusterList[cluster].PicturesListOC.Add(Picture);
                            Picture.Cluster = cluster;
                            Picture.Distance = AllDistance.Min();
                        }
                    }
                }
                //Recalculate cluster's vector
                foreach (var cluster in newClusterList)
                {
                    Vector newVector = new Vector();
                    for (int Property = 0; Property < cluster.VectorOC.Properties.Count; Property++)
                    {
                        double average = 0;
                        foreach (var Picture in cluster.PicturesListOC)
                        {
                            average += Picture.Vector.Properties[Property];
                        }
                        average = average / cluster.PicturesListOC.Count;
                        newVector.Properties.Add(average);
                    }
                    cluster.VectorOC = newVector;
                }
                //Check to end of while
                bool checkToEnd = true;
                for (int cluster = 0; cluster < ClusterList.Count; cluster++)
                {
                    if (newClusterList[cluster] != ClusterList[cluster])
                    {
                        ClusterList = newClusterList;
                        checkToEnd = false;
                    }
                }
                if (checkToEnd)
                    break;
                //Clear Pictures List of all cluster
                foreach (var cluster in newClusterList)
                {
                    cluster.PicturesListOC.Clear();
                }
            }
            return ClusterList;
        }
    }
}
