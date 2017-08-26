using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Microsoft.Win32;

namespace LiveSplit.PetThePup {
  /// <summary>
  /// Simplifies access to pup registry keys.
  /// </summary>
  public class DoggoRegistry {
    public uint CapturedAmount => (uint) this._capturedDoggos.Count;
    public bool CapturedAll => this.CapturedAmount == Enum.GetValues(typeof(Doggo)).Length;

    private const string RegistryKey = @"Software\will herring\Pet The Pup at the Party";
    private readonly List<Doggo> _capturedDoggos = new List<Doggo>();
    
    /// <summary>
    /// Computes the name of a unity registry key.
    /// </summary>
    /// <param name="text">a key name</param>
    /// <returns>a key including its hash</returns>
    private static string ComputeUnityKey(string text) {
      return text + "_h" + text.Aggregate<char, uint>(5381, (current, c) => current * 33 ^ c);
    }

    /// <summary>
    /// Deletes all game keys.
    /// </summary>
    public void DeleteKeys() {
      this._capturedDoggos.Clear();

      try {
        using (RegistryKey key = Registry.CurrentUser.OpenSubKey(RegistryKey, true)) {
          // the key itself is not defined (e.g. the game has yedt to be executed)
          if (key == null) {
            return;
          }

          foreach (Doggo value in Enum.GetValues(typeof(Doggo))) {
            Debug.Write("Deleting " + RegistryKey + "\\" + ComputeUnityKey(value.ToString()) + " ... ");

            if (key.GetValue(ComputeUnityKey(value.ToString())) == null) {
              Debug.WriteLine("NO SUCH KEY");
              continue;
            }
            
            key.DeleteValue(ComputeUnityKey(value.ToString()));
            Debug.WriteLine("OK");
          }
        }
      } catch (Exception ex) {
        Debug.Write("Failed to access registry: " + ex.Message);
      }
    }

    /// <summary>
    /// Updates the amount and set of discovered doggos.
    /// </summary>
    public void Update() {
      using (RegistryKey key = Registry.CurrentUser.OpenSubKey(RegistryKey, false)) {
        // the key itself is not defined (e.g. the game has yedt to be executed)
        if (key == null) {
          return;
        }

        try {
          foreach (Doggo value in Enum.GetValues(typeof(Doggo))) {
            if (this._capturedDoggos.Contains(value)) {
              continue;
            }

            object keyValue = key.GetValue(ComputeUnityKey(value.ToString()));

            if (keyValue != null && (int) keyValue == 1) {
              Debug.WriteLine("Key " + RegistryKey + "\\" + ComputeUnityKey(value.ToString()) + " has changed");
              this._capturedDoggos.Add(value);
            }
          }
        } catch (Exception ex) {
          Debug.Write("Failed to access registry: " + ex.Message);
        }
      }
    }
  }
}
