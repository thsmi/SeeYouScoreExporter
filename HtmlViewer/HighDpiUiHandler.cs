using System;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;

namespace net.tschmid.scooring.htmlviewer
{
    ///
    /// Makes the Webbrowser Component UI Aware https://msdn.microsoft.com/en-us/library/mt622670.aspx
    /// 


    // Definitions needed for our injection...

    // From https://msdn.microsoft.com/en-us/library/aa753277(v=vs.85).aspx
    [Flags()]
    public enum DOCHOSTUIFLAG
    {
        DOCHOSTUIFLAG_DIALOG = 0x00000001,
        DOCHOSTUIFLAG_DISABLE_HELP_MENU = 0x00000002,
        DOCHOSTUIFLAG_NO3DBORDER = 0x00000004,
        DOCHOSTUIFLAG_SCROLL_NO = 0x00000008,
        DOCHOSTUIFLAG_DISABLE_SCRIPT_INACTIVE = 0x00000010,
        DOCHOSTUIFLAG_OPENNEWWIN = 0x00000020,
        DOCHOSTUIFLAG_DISABLE_OFFSCREEN = 0x00000040,
        DOCHOSTUIFLAG_FLAT_SCROLLBAR = 0x00000080,
        DOCHOSTUIFLAG_DIV_BLOCKDEFAULT = 0x00000100,
        DOCHOSTUIFLAG_ACTIVATE_CLIENTHIT_ONLY = 0x00000200,
        DOCHOSTUIFLAG_OVERRIDEBEHAVIORFACTORY = 0x00000400,
        DOCHOSTUIFLAG_CODEPAGELINKEDFONTS = 0x00000800,
        DOCHOSTUIFLAG_URL_ENCODING_DISABLE_UTF8 = 0x00001000,
        DOCHOSTUIFLAG_URL_ENCODING_ENABLE_UTF8 = 0x00002000,
        DOCHOSTUIFLAG_ENABLE_FORMS_AUTOCOMPLETE = 0x00004000,
        DOCHOSTUIFLAG_ENABLE_INPLACE_NAVIGATION = 0x00010000,
        DOCHOSTUIFLAG_IME_ENABLE_RECONVERSION = 0x00020000,
        DOCHOSTUIFLAG_THEME = 0x00040000,
        DOCHOSTUIFLAG_NOTHEME = 0x00080000,
        DOCHOSTUIFLAG_NOPICS = 0x00100000,
        DOCHOSTUIFLAG_NO3DOUTERBORDER = 0x00200000,
        DOCHOSTUIFLAG_DISABLE_EDIT_NS_FIXUP = 0x00400000,
        DOCHOSTUIFLAG_LOCAL_MACHINE_ACCESS_CHECK = 0x00800000,
        DOCHOSTUIFLAG_DISABLE_UNTRUSTEDPROTOCOL = 0x01000000,
        DOCHOSTUIFLAG_HOST_NAVIGATES = 0x02000000,
        DOCHOSTUIFLAG_ENABLE_REDIRECT_NOTIFICATION = 0x04000000,
        DOCHOSTUIFLAG_USE_WINDOWLESS_SELECTCONTROL = 0x08000000,
        DOCHOSTUIFLAG_USE_WINDOWED_SELECTCONTROL = 0x10000000,
        DOCHOSTUIFLAG_ENABLE_ACTIVEX_INACTIVATE_MODE = 0x20000000,
        DOCHOSTUIFLAG_DPI_AWARE = 0x40000000
    }

    // From http://www.pinvoke.net/default.aspx/Interfaces/IDocHostUIHandler.html

    public enum DOCHOSTUITYPE
    {
        DOCHOSTUITYPE_BROWSE = 0,
        DOCHOSTUITYPE_AUTHOR = 1
    }

    public enum DOCHOSTUIDBLCLK
    {
        DOCHOSTUIDBLCLK_DEFAULT = 0,
        DOCHOSTUIDBLCLK_SHOWPROPERTIES = 1,
        DOCHOSTUIDBLCLK_SHOWCODE = 2
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct DOCHOSTUIINFO
    {
        public uint cbSize;
        public uint dwFlags;
        public uint dwDoubleClick;
        [MarshalAs(UnmanagedType.BStr)]
        public string pchHostCss;
        [MarshalAs(UnmanagedType.BStr)]
        public string pchHostNS;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct tagMSG
    {
        public IntPtr hwnd;
        public uint message;
        public uint wParam;
        public int lParam;
        public uint time;
        public tagPOINT pt;
    }

    // Added missing definitions of tagRECT/tagPOINT

    [StructLayout(LayoutKind.Sequential, Pack = 4)]

    public struct tagRECT

    {

        public int left;
        public int top;
        public int right;
        public int bottom;

    }

    [StructLayout(LayoutKind.Sequential, Pack = 4)]

    public struct tagPOINT

    {

        public int x;
        public int y;

    }

    [ComImport()]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    [GuidAttribute("bd3f23c0-d43e-11cf-893b-00aa00bdce1a")]
    public interface IDocHostUIHandler
    {
        [PreserveSig]
        uint ShowContextMenu(
        uint dwID,
        ref tagPOINT ppt,
        [MarshalAs(UnmanagedType.IUnknown)]  object pcmdtReserved,
        [MarshalAs(UnmanagedType.IDispatch)] object pdispReserved
        );

        void GetHostInfo(ref DOCHOSTUIINFO pInfo);

        void ShowUI(uint dwID, ref object pActiveObject, ref object pCommandTarget, ref object pFrame, ref object pDoc);

        void HideUI();

        void UpdateUI();

        void EnableModeless(int fEnable);

        void OnDocWindowActivate(int fActivate);

        void OnFrameWindowActivate(int fActivate);

        void ResizeBorder(ref tagRECT prcBorder, int pUIWindow, int fFrameWindow);

        [PreserveSig]
        uint TranslateAccelerator(ref tagMSG lpMsg, ref Guid pguidCmdGroup, uint nCmdID);

        void GetOptionKeyPath([MarshalAs(UnmanagedType.BStr)] ref string pchKey, uint dw);

        uint GetDropTarget(int pDropTarget, ref int ppDropTarget);

        [PreserveSig]
        void GetExternal([MarshalAs(UnmanagedType.IDispatch)] out object ppDispatch);

        [PreserveSig]
        uint TranslateUrl(
        uint dwTranslate,
        [MarshalAs(UnmanagedType.BStr)] string pchURLIn,
        [MarshalAs(UnmanagedType.BStr)] ref string ppchURLOut
        );

        IDataObject FilterDataObject(IDataObject pDO);
    }

    // From http://www.pinvoke.net/default.aspx/Interfaces/ICustomDoc.html

    [ComImport, InterfaceType(ComInterfaceType.InterfaceIsIUnknown), Guid("3050F3F0-98B5-11CF-BB82-00AA00BDCE0B")]
    public interface ICustomDoc
    {
        void SetUIHandler([In, MarshalAs(UnmanagedType.Interface)] IDocHostUIHandler pUIHandler);
    }

    /// <summary>
    ///   Implements a ui handler which enables a webbrowser's dpi awareness...
    /// </summary>
    public sealed  class HighDpiUiHandler : IDocHostUIHandler
    {
        private System.Windows.Forms.WebBrowser browser;

        public HighDpiUiHandler(System.Windows.Forms.WebBrowser browser)
        {
            this.browser = browser;
        }

        public IDataObject FilterDataObject(IDataObject pDO)
        {
            return pDO;
        }

        public uint GetDropTarget(int pDropTarget, ref int ppDropTarget)
        {
            ppDropTarget = pDropTarget;
            return 0;
        }


        public void GetHostInfo(ref DOCHOSTUIINFO pInfo)
        {
            // Add the DPI Aware flag, we know our app is aware... 
            pInfo.dwFlags |= (uint)(DOCHOSTUIFLAG.DOCHOSTUIFLAG_DPI_AWARE);

            // No Scrollbars...
            pInfo.dwFlags |= (uint)(DOCHOSTUIFLAG.DOCHOSTUIFLAG_SCROLL_NO);
        }

        public void ResizeBorder(ref tagRECT prcBorder, int pUIWindow, int fFrameWindow)
        {
        }


        public uint ShowContextMenu([In] uint dwID, [In] ref tagPOINT ppt, [In, MarshalAs(UnmanagedType.IUnknown)] object pcmdtReserved, [In, MarshalAs(UnmanagedType.IDispatch)] object pdispReserved)
        {
            return 0;
        }

        public void ShowUI(uint dwID, ref object pActiveObject, ref object pCommandTarget, ref object pFrame, ref object pDoc)
        {
        }

        public uint TranslateAccelerator(ref tagMSG lpMsg, ref Guid pguidCmdGroup, uint nCmdID)
        {
            return 0;
        }

        public uint TranslateUrl(uint dwTranslate, [MarshalAs(UnmanagedType.BStr)] string pchURLIn, [MarshalAs(UnmanagedType.BStr)] ref string ppchURLOut)
        {
            //ppchURLOut = pchURLIn;
            return 0;
        }


        void IDocHostUIHandler.EnableModeless(int fEnable)
        {
        }

        void IDocHostUIHandler.GetExternal(out object ppDispatch)
        {
            ppDispatch = this.browser.ObjectForScripting;
        }

        void IDocHostUIHandler.GetOptionKeyPath(ref string pchKey, uint dw)
        {
            pchKey = null;
        }

        void IDocHostUIHandler.HideUI()
        {
        }

        void IDocHostUIHandler.OnDocWindowActivate(int fActivate)
        {
        }

        void IDocHostUIHandler.OnFrameWindowActivate(int fActivate)
        {
        }

        void IDocHostUIHandler.UpdateUI()
        {
        }

        public void Inject()
        {
            ((ICustomDoc)(browser.Document.DomDocument)).SetUIHandler(this);
        }


    }
    
}
