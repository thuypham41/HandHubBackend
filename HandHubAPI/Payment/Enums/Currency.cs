﻿using System.ComponentModel;

namespace HandHubAPI.Payment.Enums
{
    /// <summary>
    /// Đơn vị tiền tệ sử dụng cho giao dịch
    /// </summary>
    public enum Currency
    {
        [Description("Việt Nam đồng")]
        VND,

        //[Description("Đô la Mỹ")]
        //USD,
    }
}
