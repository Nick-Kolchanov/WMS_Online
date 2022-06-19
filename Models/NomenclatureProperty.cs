using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace WMS_Online.Models
{
    public partial class NomenclatureProperty
    {
        public int? NomenclatureId { get; set; }
        public int? PropertyId { get; set; }
        public string Value { get; set; } = "";

        public virtual Nomenclature? Nomenclature { get; set; }
        public virtual Property? Property { get; set; }
    }
}
