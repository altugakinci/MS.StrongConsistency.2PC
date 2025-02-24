using Coordinator.Enums;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Coordinator.Models;

public class NodeState
{
    public Guid Id { get; set; }
    public Guid TransactionId { get; set; }
    public ReadyStates ReadyState { get; set; }
    public TransactionStates TransactionState { get; set; }
    
    public Node Node { get; set; }

}
