using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Coordinator.Models;

public class Node
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Name { get; set; }

    public ICollection<NodeState> NodeStates { get; set;}
}
