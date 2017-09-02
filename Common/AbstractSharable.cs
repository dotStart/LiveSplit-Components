using System;
using System.Diagnostics;

namespace LiveSplit.dotStart.Common {
  /// <summary>
  /// Provides an abstract sharable implementation.
  /// </summary>
  public abstract class AbstractSharable : ISharable {
    public event EventHandler OnDelete;

    /// <summary>
    /// Stores the total amount of references to this memory instance.
    /// </summary>
    private uint _referenceCount;

    /// <inheritdoc />
    public void Claim() {
      ++this._referenceCount;
    }
    
    /// <inheritdoc />
    public void Dispose() {
      if (this._referenceCount == 0) {
        throw new InvalidOperationException("Illegal dispose call: Exceeded amount of known owners");
      }
      
      if (--this._referenceCount == 0) {
        Debug.WriteLine("[Abstract Sharable] Last reference disposed - Destroying instance");
        this.OnDelete?.Invoke(this, EventArgs.Empty);
      }
    }
  }
}
