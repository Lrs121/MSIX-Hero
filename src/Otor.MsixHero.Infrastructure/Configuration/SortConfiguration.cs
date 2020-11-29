﻿using System.Runtime.Serialization;

namespace Otor.MsixHero.Infrastructure.Configuration
{
    public class PackagesSortConfiguration : BaseJsonSetting
    {
        public PackagesSortConfiguration()
        {
            this.SortingMode = PackageSort.Name;
            this.Descending = false;
        }

        [DataMember(Name= "sortingMode")]
        public PackageSort SortingMode { get; set; }

        [DataMember(Name = "descending")]
        public bool Descending { get; set; }
    }
}