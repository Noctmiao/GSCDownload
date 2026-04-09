using AntdUI;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace GuidanceStsbilityCommsDownLoad
{
    public enum DataType
    {
        typeDataUnknown = 0,
        typeDataCurve = 1,
        typeDataSurvey = 2,
        typeDataRt = 3,
        typeDataTops = 4,

        typeDataMapNorthEast = 5,   // north, east coordinates on a map

        typeDataMapPoints = 6,   // map object: points
        typeDataMapPolyLine = 7,   // map object: polyline
        typeDataMapArc = 8,   // map object: arc

        typeDataActualSurvey = 9,
        typeDataInterpSurvey = 10,

        typeDataGeologyBed = 11,
        typeDataMarkerBed = 12,
        //typeDataImage           = 13,

        typeDataMwd = 14,
        typeDataGamma = 15,
        typeDataResistivity = 16,
        //typeDataDirRes          = 17,
        typeDataImageDirRes = 17,
        typeDataNBBatteryStatus = 18,
        typeDataNBResistivity = 19,
        typeDataNBGamma = 20,
        typeDataImageNBGamma = 21,
        typeDataNBOthers = 22,       // TEM, INC, RPM
        typeDataPwd = 23,

        typeDataDNDensity = 30,
        typeDataDNSonic = 31,
        typeDataDNNeutron = 32,

        typeDataRSShorthop = 33,
        typeDataDNMaster = 34,
        typeDataRSDownlink = 35,
    };
    public enum DataMergeOption
    {
        leave,
        replaceAll,
        replacePart,        // delete data1 in [strat2, end2], add all data2 to data1
        replaceFrom,        // delete data1 in [start2, end1], add all data2 to data1
        append,
        appendAll,          // add all data2 to data1
        specifyName,
    }
    public enum DepthType
    {
        typeDepthTVD,
        typeDepthMD,
        typeDepthTST,
        typeDepthTVT,
        typeTime,
    }
    /// <summary>
    /// This flag indicates how to interpolate value at depth between 
    /// top and bottom. The default is using a straight line connect
    /// the top and bottom points.
    /// </summary>
    public enum InterpolateFlag
    {
        flagInterpolateLine,
        flagInterpolateUseStartValue,
        flagInterpolateUseEndValue,
        flagInterpolateHalfStartHalfEnd,
        flagInterpolateOther,
    }
    public class Dataset : IComparable<Dataset>
    {
        public const double NO_READING = -999.25;
        public static bool IsNoReading(double dValue)
        {
            return (Math.Abs(dValue - NO_READING) < 0.0001);
        }

        // primary properties
        public String Name;
        public DataType DataType;
        public int TotalColumns;
        public String[] ColumnNames;

        private List<ADataPoint> dataPointList;
        public int TotalRecords
        {
            get { return (dataPointList == null) ? 0 : dataPointList.Count; }
        }
        public int DataID;
        public bool Sortable;

        // secondary properties
        public DepthType DepthType;
        public InterpolateFlag InterpolateFlag;
        public String DepthUnit;
        public String ValueUnit;
        public String Comment;

        // derived properties
        public double StartDepth;
        public double EndDepth;
        public double Step;             // (step == 0) <==> not regular step
        public double MinValue;
        public double MaxValue;
        public Dataset()
        {
            Name = String.Empty;
            DataType = DataType.typeDataCurve;
            DepthType = DepthType.typeDepthMD;
            DepthUnit = String.Empty;
            ValueUnit = String.Empty;
            TotalColumns = 2;
            ColumnNames = new String[TotalColumns];
            ColumnNames[0] = "Depth";
            ColumnNames[1] = "Value";
            DataID = 0;
            Sortable = true;
            dataPointList = null;
            StartDepth = 0.0;
            EndDepth = 0.0;
            Step = 0.0;
            InterpolateFlag = InterpolateFlag.flagInterpolateLine;
            Comment = "";
        }
        public Dataset(Dataset ds)
        {
            Name = ds.Name;
            DataType = ds.DataType;
            DepthType = ds.DepthType;
            DepthUnit = ds.DepthUnit;
            ValueUnit = ds.ValueUnit;
            if (ds.TotalColumns > 0 && ds.ColumnNames != null && ds.ColumnNames.Length >= ds.TotalColumns)
            {
                ColumnNames = new String[ds.TotalColumns];
                for (int i = 0; i < ds.TotalColumns; i++) ColumnNames[i] = ds.ColumnNames[i];
            }

            int totalRecords = 0;
            double[] data = null;
            ds.GetData(out totalRecords, out data);
            SetData(ds.TotalColumns, totalRecords, data);
            DataID = (int)ds.DataID;
            Sortable = true;
            InterpolateFlag = ds.InterpolateFlag;
            Comment = ds.Comment;
        }
        public void GetData(out int nTotalRecords, out double[] data)
        {
            nTotalRecords = TotalRecords;
            data = new double[nTotalRecords * TotalColumns];
            //Array.Copy(m_dData, data, m_nTotalRecords * m_nTotalColumns);
            for (int i = 0; i < nTotalRecords; i++)
            {
                for (int j = 0; j < TotalColumns; j++)
                {
                    data[i * TotalColumns + j] = dataPointList[i][j];
                }
            }
        }
        public List<ADataPoint> GetData()
        {
            return dataPointList;
        }
        // used when to reset # of cols
        public void SetData(int nTotalColumns, int nTotalRecords, double[] data)
        {
            //if (data == null || nTotalRecords <= 0 || nTotalColumns <= 0)
            if (data == null || nTotalRecords < 0 || nTotalColumns < 0)
            {
                return;
            }
            TotalColumns = nTotalColumns;
            SetData(nTotalRecords, data);
        }
        // used when input data has the same # of cols as the object data 
        public void SetData(int nTotalRecords, double[] data)
        {
            //if (data == null || nTotalRecords <= 0)
            if (data == null || nTotalRecords < 0)
            {
                //totalRecords = 0;
                dataPointList = null;
                return;
            }

            int totalRecords = nTotalRecords;
            dataPointList = new List<ADataPoint>(totalRecords);
            for (int i = 0; i < totalRecords; i++)
            {
                double[] tempPoint = new double[TotalColumns];
                for (int j = 0; j < TotalColumns; j++)
                {
                    tempPoint[j] = data[TotalColumns * i + j];
                }
                dataPointList.Add(new ADataPoint(tempPoint));
            }

            CalculateDerivedProperties();
        }
        public void SetData(List<ADataPoint> dpList)
        {
            dataPointList = dpList;
            CalculateDerivedProperties();
        }

        public void CalculateDerivedProperties()
        {
            Sort();

            StartDepth = EndDepth = MinValue = MaxValue = Dataset.NO_READING;
            bool bRegularStep = true;
            Step = 0;
            double dLastDepth = Dataset.NO_READING;
            for (int i = 0; i < TotalRecords; i++)
            {
                double dDepth = dataPointList[i][0];
                double dValue = dataPointList[i][1];
                StartDepth = (Dataset.IsNoReading(StartDepth)) ? dDepth : Math.Min(StartDepth, dDepth);
                EndDepth = (Dataset.IsNoReading(EndDepth)) ? dDepth : Math.Max(EndDepth, dDepth);
                if (Dataset.IsNoReading(dValue) == false)
                {
                    MinValue = (Dataset.IsNoReading(MinValue)) ? dValue : Math.Min(MinValue, dValue);
                    MaxValue = (Dataset.IsNoReading(MaxValue)) ? dValue : Math.Max(MaxValue, dValue);
                }

                if (bRegularStep == true)
                {
                    if (Dataset.IsNoReading(dLastDepth) == false && Dataset.IsNoReading(dDepth) == false)
                    {
                        double dStep = dDepth - dLastDepth;
                        if (Step > 0)
                        {
                            bRegularStep = (Math.Abs(Step - dStep) < 0.0001);
                            if (bRegularStep == false)
                            {
                                Step = 0;
                            }
                        }
                        else
                        {
                            Step = dStep;
                        }
                    }

                    dLastDepth = dDepth;
                }
            }
        }
        public void Sort()
        {
            if (Sortable == true) Sort(0, true);
        }
        public void Sort(int nKeyCol, bool bIncrease)
        {
            if (Sortable == false || dataPointList == null) return;

            ADataPoint.DataPointComparer comparer = ADataPoint.GetComparer();
            comparer.KeyIndex = nKeyCol;
            dataPointList.Sort(comparer);
            if (bIncrease == false)
            {
                dataPointList.Reverse();
            }
        }
        public int CompareTo(Dataset rhs)
        {
            return Name.CompareTo(rhs.Name);
        }
        public void AppendData(ADataPoint newDataPoint)
        {
            if (newDataPoint == null) return;

            if (dataPointList == null) dataPointList = new List<ADataPoint>();

            dataPointList.Add(newDataPoint);
        }
    }

    [Serializable]
    public class ADataPoint : IComparable<ADataPoint>
    {
        // private member is a double array
        private double[] m_dataPoint;
        // public member
        public double[] DataPoint
        {
            get { return m_dataPoint; }
            set { m_dataPoint = value; }
        }
        // constructor
        public ADataPoint(double[] dataPoint)
        {
            m_dataPoint = new double[dataPoint.Length];
            Array.Copy(dataPoint, m_dataPoint, dataPoint.Length);
        }
        // indexer
        public double this[int j]
        {
            get
            {
                if (j < 0 || j >= m_dataPoint.Length)
                {
                    return Dataset.NO_READING;
                }
                else
                {
                    return m_dataPoint[j];
                }
            }
            set
            {
                if (j < 0 || j >= m_dataPoint.Length)
                {
                    ;
                }
                else
                {
                    m_dataPoint[j] = value;
                }
            }
        }
        public int CompareTo(ADataPoint rhs)
        {
            return m_dataPoint[0].CompareTo(rhs.m_dataPoint[0]);
        }
        public int CompareTo(ADataPoint rhs, int keyIndex)
        {
            return this.m_dataPoint[keyIndex].CompareTo(rhs.m_dataPoint[keyIndex]);
        }

        // static method to get a Comparer object
        public static DataPointComparer GetComparer()
        {
            return new ADataPoint.DataPointComparer();
        }
        // nested class which implements IComparer
        public class DataPointComparer : IComparer<ADataPoint>
        {
            private int m_keyIndex;

            public int KeyIndex
            {
                get { return m_keyIndex; }
                set { m_keyIndex = value; }
            }
            public bool Equals(ADataPoint lhs, ADataPoint rhs)
            {
                return (this.Compare(lhs, rhs) == 0);
            }

            public int Compare(ADataPoint lhs, ADataPoint rhs)
            {
                return lhs.CompareTo(rhs, KeyIndex);
            }
        }
    }
    
    public static class DatasetUtility
    {
        //Note: the merged result is NOT sorted.
        public static void Merge(List<ADataPoint> dataPointList1,
            List<ADataPoint> dataPointList2, DataMergeOption mergeOption)
        {
            if (dataPointList2 == null || dataPointList2.Count == 0)
            {
                return;
            }

            if (dataPointList1 == null) dataPointList1 = new List<ADataPoint>();

            switch (mergeOption)
            {
                case DataMergeOption.replaceAll:
                    //dataPointList1 = dataPointList2;
                    dataPointList1.Clear();
                    dataPointList1.AddRange(dataPointList2);
                    break;
                case DataMergeOption.replacePart:
                    dataPointList1.Sort();
                    dataPointList2.Sort();
                    int numberOfRecords2 = dataPointList2.Count;
                    double startDepth2 = dataPointList2[0][0];
                    double endDepth2 = dataPointList2[numberOfRecords2 - 1][0];
                    int startIndex = -1;
                    int endIndex = -1;
                    for (int i = 0; i < dataPointList1.Count; i++)
                    {
                        double depth = dataPointList1[i][0];
                        if (depth >= startDepth2 && startIndex == -1)
                        {
                            startIndex = i;
                        }
                        //if (depth <= endDepth2 && endIndex == -1)
                        if (depth <= endDepth2)
                        {
                            endIndex = i;
                        }
                    }

                    //list2 is all deeper than list1, endIndex = n-1
                    if (startIndex == -1) startIndex = dataPointList1.Count;//startIndex = 0;
                    //list2 is all shallower than list1, startIndex = 0
                    if (endIndex == -1) endIndex = -1;//dataPointList1.Count - 1;
                    if (startIndex >= 0 && endIndex >= 0 && startIndex <= endIndex)
                    {
                        dataPointList1.RemoveRange(startIndex, endIndex - startIndex + 1);
                        dataPointList1.InsertRange(startIndex, dataPointList2);
                    }
                    else//list1 is empty
                    {
                        dataPointList1.AddRange(dataPointList2);
                    }
                    break;
                case DataMergeOption.replaceFrom:
                    dataPointList1.Sort();
                    dataPointList2.Sort();
                    if (dataPointList1.Count > 0)
                    {
                        int start = -1;
                        for (int i = 0; i < dataPointList1.Count; i++)
                        {
                            double depth = dataPointList1[i][0];
                            if (depth >= dataPointList2[0][0])
                            {
                                start = i;
                                break;
                            }
                        }
                        if (start == -1) start = dataPointList1.Count;
                        dataPointList1.RemoveRange(start, dataPointList1.Count - start);
                        dataPointList1.InsertRange(start, dataPointList2);
                    }
                    else dataPointList1.AddRange(dataPointList2);
                    break;
                case DataMergeOption.append:
                    dataPointList1.Sort();
                    int numberOfRecords1 = dataPointList1.Count;
                    if (dataPointList1.Count > 0)
                    {
                        double startDepth1 = dataPointList1[0][0];
                        double endDepth1 = dataPointList1[numberOfRecords1 - 1][0];

                        foreach (ADataPoint dp in dataPointList2)
                        {
                            double depth = dp[0];
                            //if (depth < startDepth1 || depth > endDepth1)
                            if (depth > endDepth1)
                            {
                                dataPointList1.Add(dp);
                            }
                        }
                    }
                    else
                    {
                        //empty list1, replace by list2
                        dataPointList1.AddRange(dataPointList2);
                    }
                    break;
                case DataMergeOption.appendAll:
                    if (dataPointList1.Count > 0)
                    {
                        double minDistance = 0.01;      // append all list2, except those of duplicated depth
                        int isBadColIndex = 3;          // when at same depth, but existing record isBad, new record is allowed to append
                        foreach (ADataPoint dp in dataPointList2)
                        {
                            double depth = dp[0];
                            bool found = false;         // if good record close to depth is found
                            for (int i = 0; i < dataPointList1.Count; i++)
                            {
                                ADataPoint dp1 = dataPointList1[i];
                                bool isGood = true;     // use isGood for easy understand
                                if (dp1.DataPoint.Length > isBadColIndex && Math.Abs(dp1[isBadColIndex] - 1) < 0.01) isGood = false;
                                if (Math.Abs(depth - dp1[0]) < minDistance && isGood == true)
                                {   // good record close to depth is found
                                    found = true;
                                    break;
                                }
                            }

                            if (found == false)
                            {
                                dataPointList1.Add(dp);
                            }
                        }
                    }
                    else
                    {
                        // empty list1, add all list2
                        dataPointList1.AddRange(dataPointList2);
                    }
                    break;
                default:
                    break;
            }
        }
        public static void Merge(List<Dataset> datasetList1, List<Dataset> datasetList2,
            DataMergeOption mergeOption)
        {
            if (datasetList1 == null || datasetList2 == null) return;

            foreach (Dataset dataset in datasetList2)
            {
                if (dataset == null) continue;
                Dataset ds = DatasetUtility.FindDatasetInList(dataset.Name, datasetList1);
                if (ds == null)
                {
                    ds = new Dataset(dataset);
                    datasetList1.Add(ds);
                }
                else
                {
                    if (ds.Name == "IDirRes" && mergeOption == DataMergeOption.append)
                    {   // allow DirRes measurement when tool string is lifting up
                        DatasetUtility.Merge(ds, dataset, DataMergeOption.replacePart);
                    }
                    else DatasetUtility.Merge(ds, dataset, mergeOption);
                }
            }
        }
        public static bool Merge(Dataset dataset1, Dataset dataset2,
            DataMergeOption mergeOption)
        {
            if (dataset1 == null || dataset2 == null)
            {
                return false;
            }

            // if merge data, check if the dataset properties match
            if (//dataset1.Name != dataset2.Name ||
                dataset1.DataType != dataset2.DataType ||
                dataset1.DepthType != dataset2.DepthType ||
                //dataset1.DepthUnit != dataset2.DepthUnit ||
                //dataset1.ValueUnit != dataset2.ValueUnit ||
                dataset1.TotalColumns != dataset2.TotalColumns)
            {
                return false;
            }

            List<ADataPoint> dataPointList1 = dataset1.GetData();
            if (dataPointList1 == null) dataPointList1 = new List<ADataPoint>();
            Merge(dataPointList1, dataset2.GetData(), mergeOption);
            dataset1.SetData(dataPointList1);

            //Merge(dataset1.GetData(), dataset2.GetData(), mergeOption);
            //dataset1.CalculateDerivedProperties();

            return true;
        }
        /// <summary>
        /// look for a dataset in the datasetList with the given name.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="datasetList"></param>
        /// <returns>null if not found, return the first dataset with the given name</returns>
        public static Dataset FindDatasetInList(String name, List<Dataset> datasetList)
        {
            if (datasetList == null) return null;

            foreach (Dataset dataset in datasetList)
            {
                if (dataset != null && String.Compare(name, dataset.Name, true) == 0) return dataset;
            }

            return null;
        }

    }
}
