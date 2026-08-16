using System;

namespace SadrScales.Integration.Sales
{
    /// <summary>
    /// One accepted sale row from the read-only <c>dbo.SADR_Logs</c> feed.
    /// </summary>
    public sealed class SadrSaleRow
    {
        internal SadrSaleRow(
            long id,
            int deviceNo,
            string? identify,
            DateTime dateTime,
            int fid,
            int sid,
            int salesman,
            int subId,
            double totalPrice,
            int plu,
            int itemClass,
            int department,
            double amount,
            int unit,
            int logType,
            double tax,
            string? text1,
            string? text2,
            string? text3,
            string? text4,
            double unitPrice,
            int coFid,
            string pluName)
        {
            Id = id;
            DeviceNo = deviceNo;
            Identify = identify;
            DateTime = dateTime;
            Fid = fid;
            Sid = sid;
            Salesman = salesman;
            SubId = subId;
            TotalPrice = totalPrice;
            Plu = plu;
            ItemClass = itemClass;
            Department = department;
            Amount = amount;
            Unit = unit;
            LogType = logType;
            Tax = tax;
            Text1 = text1;
            Text2 = text2;
            Text3 = text3;
            Text4 = text4;
            UnitPrice = unitPrice;
            CoFid = coFid;
            PluName = pluName;
        }

        /// <summary>Gets the monotonically increasing feed identity used for cursor ordering.</summary>
        public long Id { get; }
        /// <summary>Gets the scale identifier.</summary>
        public int DeviceNo { get; }
        /// <summary>Gets the optional source identification text.</summary>
        public string? Identify { get; }
        /// <summary>Gets the sale row date/time.</summary>
        public DateTime DateTime { get; }
        /// <summary>Gets the invoice FID.</summary>
        public int Fid { get; }
        /// <summary>Gets the SID.</summary>
        public int Sid { get; }
        /// <summary>Gets the salesman number.</summary>
        public int Salesman { get; }
        /// <summary>Gets the invoice row SubID.</summary>
        public int SubId { get; }
        /// <summary>Gets the total price value stored in the feed.</summary>
        public double TotalPrice { get; }
        /// <summary>Gets the PLU number.</summary>
        public int Plu { get; }
        /// <summary>Gets the class value stored in the feed.</summary>
        public int ItemClass { get; }
        /// <summary>Gets the department value.</summary>
        public int Department { get; }
        /// <summary>Gets the sold amount/weight value.</summary>
        public double Amount { get; }
        /// <summary>Gets the unit code.</summary>
        public int Unit { get; }
        /// <summary>Gets the log type.</summary>
        public int LogType { get; }
        /// <summary>Gets the tax value.</summary>
        public double Tax { get; }
        /// <summary>Gets text field 1.</summary>
        public string? Text1 { get; }
        /// <summary>Gets text field 2.</summary>
        public string? Text2 { get; }
        /// <summary>Gets text field 3.</summary>
        public string? Text3 { get; }
        /// <summary>Gets text field 4.</summary>
        public string? Text4 { get; }
        /// <summary>Gets the unit price.</summary>
        public double UnitPrice { get; }
        /// <summary>Gets the CoFID value.</summary>
        public int CoFid { get; }
        /// <summary>Gets the PLU name.</summary>
        public string PluName { get; }
    }
}
