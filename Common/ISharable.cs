using System;

namespace LiveSplit.dotStart.Common {
  /// <summary>
  /// Provides a base to instances which may be shared between multiple components.
  /// 
  /// This interface is meant to be used with implementations that internally use reference counts
  /// in order to decide whether to free resources that keep the instance alive or not (such as
  /// threads).
  /// </summary>
  public interface ISharable : IDisposable {

    /// <summary>
    /// Claims ownership of an instance.
    /// 
    /// When called the internal reference count is incremented. In order to free the respective
    /// resource, the instance which claimed ownership must call <see cref="IDisposable.Dispose"/>.
    /// </summary>
    void Claim();
  }
}
