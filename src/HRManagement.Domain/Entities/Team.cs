namespace HRManagement.Domain.Entities;
public class Team
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public Guid? ManagerId { get; set; }
    public Employee? Manager { get; set; }
    public ICollection<Employee> Members { get; set; } = new List<Employee>();
}
