using System;
using System.Diagnostics;

namespace LiveSplit.dotStart.Common.Splitter {
  /// <summary>
  /// Provides a sharable game memory implementation which relies on a reference counter in order
  /// to shut down the backing thread when necessary.
  /// </summary>
  public abstract class SharableGameMemory : GameMemory, ISharable {

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
        Debug.WriteLine("[Sharable Game Memory] Last reference disposed - Destroying remaining threads");

        if (this.Alive) {
          this.Stop();
        }
      }
    }
  }
}
