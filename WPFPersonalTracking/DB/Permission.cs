using System;
using System.Collections.Generic;

namespace WPFPersonalTracking.DB;

public partial class Permission
{
    public int Id { get; set; }

    public int EmployeeId { get; set; }

    public int UserNo { get; set; }

    public DateTime? StartDate { get; set; }

    public DateTime? EndDate { get; set; }

    public int PermissionState { get; set; }

    public string? Explanation { get; set; }

    public int PermissionAmount { get; set; }

    public virtual Employee Employee { get; set; } = null!;

    public virtual Permissionstate PermissionStateNavigation { get; set; } = null!;
}
