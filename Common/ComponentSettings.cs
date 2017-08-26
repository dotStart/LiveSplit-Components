using System;
using System.Windows.Forms;
using System.Xml;

namespace LiveSplit.dotStart.Common {
  /// <summary>
  /// Provides a base to objects which represent and allow mutation of component settings.
  /// </summary>
  public abstract class ComponentSettings : UserControl {

    /// <summary>
    /// Converts the current state of this object into a single XML node.
    /// </summary>
    /// <param name="document">the document in which the new node resides</param>
    /// <returns>an XML node</returns>
    public virtual XmlNode GetSettings(XmlDocument document) {
      XmlElement root = document.CreateElement("Settings");
      this.GetSettings(document, root);
      return root;
    }

    /// <summary>
    /// Copies the value represented by this type into a dedicated XML element.
    /// </summary>
    /// <param name="document"></param>
    /// <param name="root"></param>
    protected virtual void GetSettings(XmlDocument document, XmlElement root) {
    }

    /// <summary>
    /// Copies the values represented by a passed XML node into the local object state.
    /// </summary>
    /// <param name="settings">an XML node</param>
    public abstract void SetSettings(XmlNode settings);

    /// <summary>
    /// Reads a boolean value from the specified node within the passed element.
    /// </summary>
    /// <param name="root">a root or origin node</param>
    /// <param name="name">an element name</param>
    /// <param name="defaultValue">a default value</param>
    /// <returns>the node value</returns>
    protected static bool ReadElement(XmlNode root, string name, bool defaultValue) {
      bool value = defaultValue;

      if (root[name] != null) {
        bool.TryParse(root[name].InnerText, out value);
      }

      return value;
    }

    /// <summary>
    /// Reads an enum value from the specified node within the passed element.
    /// </summary>
    /// <param name="root">a root or origin node</param>
    /// <param name="name">an element name</param>
    /// <param name="defaultValue">a default value</param>
    /// <typeparam name="V">an enum type</typeparam>
    /// <returns>the node value</returns>
    /// <exception cref="ArgumentException">when the passed type is not an enum</exception>
    protected static V ReadElement<V>(XmlNode root, string name, V defaultValue)
      where V : struct, IConvertible {
      if (!typeof(V).IsEnum) {
        throw new ArgumentException("V must be an numerated type");
      }

      V value = defaultValue;

      if (root[name] != null) {
        Enum.TryParse(root[name].InnerText, out value);
      }

      return value;
    }

    /// <summary>
    /// Converts the passed name and value into a standard XML node.
    /// </summary>
    /// <param name="document">the document in which the new node resides</param>
    /// <param name="name">the element name</param>
    /// <param name="value">the current element value</param>
    /// <typeparam name="V">a value type</typeparam>
    /// <returns></returns>
    protected static XmlElement WriteElement<V>(XmlDocument document, string name, V value) {
      XmlElement element = document.CreateElement(name);
      element.InnerText = value.ToString();
      return element;
    }
  }
}
