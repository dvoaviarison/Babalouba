// Decompiled with JetBrains decompiler
// Type: BABALOUBA.Properties.Resources
// Assembly: BABALOUBA, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 36344566-A3BA-419C-9F40-383BB3344FF3
// Assembly location: C:\Users\Dvoaviarison\Downloads\BABALOUBA 1.1.exe

using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.Resources;
using System.Runtime.CompilerServices;

namespace Babalouba.Properties
{
  [DebuggerNonUserCode]
  [CompilerGenerated]
  [GeneratedCode("System.Resources.Tools.StronglyTypedResourceBuilder", "4.0.0.0")]
  internal class Resources
  {
    private static ResourceManager resourceMan;
    private static CultureInfo resourceCulture;

    internal Resources()
    {
    }

    [EditorBrowsable(EditorBrowsableState.Advanced)]
    internal static ResourceManager ResourceManager
    {
      get
      {
        if (object.ReferenceEquals((object) Resources.resourceMan, (object) null))
          Resources.resourceMan = new ResourceManager("BABALOUBA.Properties.Resources", typeof (Resources).Assembly);
        return Resources.resourceMan;
      }
    }

    [EditorBrowsable(EditorBrowsableState.Advanced)]
    internal static CultureInfo Culture
    {
      get
      {
        return Resources.resourceCulture;
      }
      set
      {
        Resources.resourceCulture = value;
      }
    }

    internal static Bitmap add
    {
      get
      {
        return (Bitmap) Resources.ResourceManager.GetObject(nameof (add), Resources.resourceCulture);
      }
    }

    internal static Bitmap browse
    {
      get
      {
        return (Bitmap) Resources.ResourceManager.GetObject(nameof (browse), Resources.resourceCulture);
      }
    }

    internal static Bitmap export
    {
      get
      {
        return (Bitmap) Resources.ResourceManager.GetObject(nameof (export), Resources.resourceCulture);
      }
    }

    internal static Bitmap full
    {
      get
      {
        return (Bitmap) Resources.ResourceManager.GetObject(nameof (full), Resources.resourceCulture);
      }
    }

    internal static Icon icon
    {
      get
      {
        return (Icon) Resources.ResourceManager.GetObject(nameof (icon), Resources.resourceCulture);
      }
    }

    internal static Bitmap import
    {
      get
      {
        return (Bitmap) Resources.ResourceManager.GetObject(nameof (import), Resources.resourceCulture);
      }
    }

    internal static Bitmap mute
    {
      get
      {
        return (Bitmap) Resources.ResourceManager.GetObject(nameof (mute), Resources.resourceCulture);
      }
    }

    internal static Bitmap next
    {
      get
      {
        return (Bitmap) Resources.ResourceManager.GetObject(nameof (next), Resources.resourceCulture);
      }
    }

    internal static Bitmap pause
    {
      get
      {
        return (Bitmap) Resources.ResourceManager.GetObject(nameof (pause), Resources.resourceCulture);
      }
    }

    internal static Bitmap play
    {
      get
      {
        return (Bitmap) Resources.ResourceManager.GetObject(nameof (play), Resources.resourceCulture);
      }
    }

    internal static Bitmap previous
    {
      get
      {
        return (Bitmap) Resources.ResourceManager.GetObject(nameof (previous), Resources.resourceCulture);
      }
    }

    internal static Bitmap remove
    {
      get
      {
        return (Bitmap) Resources.ResourceManager.GetObject(nameof (remove), Resources.resourceCulture);
      }
    }

    internal static Bitmap speed
    {
      get
      {
        return (Bitmap) Resources.ResourceManager.GetObject(nameof (speed), Resources.resourceCulture);
      }
    }

    internal static Bitmap speedPlus
    {
      get
      {
        return (Bitmap) Resources.ResourceManager.GetObject(nameof (speedPlus), Resources.resourceCulture);
      }
    }

    internal static Bitmap stop
    {
      get
      {
        return (Bitmap) Resources.ResourceManager.GetObject(nameof (stop), Resources.resourceCulture);
      }
    }

    internal static Bitmap volume
    {
      get
      {
        return (Bitmap) Resources.ResourceManager.GetObject(nameof (volume), Resources.resourceCulture);
      }
    }
  }
}
