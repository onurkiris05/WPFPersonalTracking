using System;
using System.Collections.Generic;

namespace WPFPersonalTracking.DB;

public partial class Salarymonth
{
    public int Id { get; set; }

    public string MonthName { get; set; } = null!;

    public virtual ICollection<Salary> Salaries { get; set; } = new List<Salary>();
}
