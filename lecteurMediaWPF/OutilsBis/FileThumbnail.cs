using Microsoft.VisualBasic;
using System;
using System.Collections;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;

using System.Runtime.InteropServices;
using prjInterfaces;
using System.IO;
using System.Text;

public class FileThumbnail : IDisposable
{
    static void Main(string[] args)
    {
    }

    [DllImport("shell32.dll", CharSet = CharSet.Unicode)]
    private static extern int SHGetDesktopFolder(out IShellFolder ppDesktopFolder);
    [DllImport("shell32.dll", CharSet = CharSet.Unicode)]
    private static extern int SHGetPathFromIDList(
               IntPtr pidl,
               StringBuilder pszPath);
    [DllImport("shell32.dll", EntryPoint = "#157", CharSet = CharSet.Unicode)]
    private static extern IntPtr ILCreateFromPath(
           [MarshalAs(UnmanagedType.LPTStr)]string lpszPath);
    [DllImport("shell32.dll", EntryPoint = "#155", CharSet = CharSet.Unicode)]
    private static extern int ILFree(
          IntPtr pidl);
    [DllImport("shell32.dll", EntryPoint = "#18", CharSet = CharSet.Unicode)]
    private static extern IntPtr ILClone(
          IntPtr pidl);
    [DllImport("shell32.dll", EntryPoint = "#16", CharSet = CharSet.Unicode)]
    private static extern IntPtr ILFindLastID(
          IntPtr pidl);
    [DllImport("shell32.dll", EntryPoint = "SHGetFileInfoW", CharSet = CharSet.Unicode)]
    private static extern int SHGetFileInfo(
       string pszPath,
       int dwFileAttributes,
       ref SHFILEINFO psfi,
       int cbFileInfo,
       SHGFIFLAGS uFlags);
    [DllImport("gdi32.dll")]
    static extern bool DeleteObject(IntPtr hObject);

    //==========================================================
    //renvoie un IShellFolder correspondant au dossier parent de szFileName
    //==========================================================
    //IN szFileName : nom de fichier complet
    //OUT pidl : pointeur vers un ITEMLIST contenant le nom de fichier de szFilename
    //renvoie une instance de IShellFolder
    //==========================================================
    private IShellFolder GetShellFolder(string szFileName, ref IntPtr pidl)
    {
        IShellFolder folder = null;
        IShellFolder item = null;
        string szFile;
        string szPath;
        int cEaten = 0;
        int pdwAttrib = 0;
        IntPtr abspidl;

        //initialisation du guid de IShellFolder
        //iid = "{000214E6-0000-0000-C000-000000000046}"
        Guid uuidIShellFolder = new Guid("000214E6-0000-0000-C000-000000000046");

        //récupèration deu bureau
        SHGetDesktopFolder(out folder);

        //si c'est un lecteur seul, on le base sur "My Computer" (enfin, son guid)
        if (szFileName.Length == 3)
        {
            szFileName = System.IO.Path.GetFullPath(szFileName);
            szPath = "::{20D04FE0-3AEA-1069-A2D8-08002B30309D}";
            szFile = szFileName;
            //sinon, on sépare dossier parent et nom de fichier
        }
        else
        {
            szPath = Path.GetDirectoryName(szFileName);
            szFile = Path.GetFileName(szFileName);
        }

        //on parse le nom de dossier parent
        folder.ParseDisplayName(IntPtr.Zero, IntPtr.Zero, szPath, out cEaten, out pidl, IntPtr.Zero);
        //on crée un objet séparé (du bureau) pour le dossier parent
        item = folder.BindToObject(pidl, IntPtr.Zero, ref uuidIShellFolder);
        ILFree(pidl);

        //on calcule l'ITEMLIST du nom de fichier seul (sans la nom de dossier parent)
        abspidl = ILCreateFromPath(szFileName);
        pidl = ILFindLastID(abspidl);
        pidl = ILClone(pidl);
        ILFree(abspidl);

        return item;
    }

    //==========================================================
    //renvoie une instance de l'interface d'extraction de miniature pour item
    //==========================================================
    //IN item : instance de IShellFolder depuis laquelle on extrait l'extracteur de miniature
    //IN pidl : pointeur vers un ITEMLIST contenant le nom de fichier dont on veut la miniature
    //renvoie une instance de IExtractImage
    //==========================================================
    private IExtractImage getThumbnail(IShellFolder item, IntPtr pidl)
    {
        int prgf = 0;
        //Dim iid As String
        Guid uuidIExtractImage = new Guid("BB2E617C-0920-11D1-9A0B-00C04FC2D6C1");

        //init du guid
        //iid = "{BB2E617C-0920-11D1-9A0B-00C04FC2D6C1}"

        try
        {
            //si on a un pidl de fichier
            if (pidl != IntPtr.Zero)
            {
                //on extrait la miniature de fichier
                return (IExtractImage)item.GetUIObjectOf(IntPtr.Zero, 1, ref pidl, ref uuidIExtractImage, out prgf);
            }
            else
            {
                //sinon du dossier parent
                return (IExtractImage)item.CreateViewObject(IntPtr.Zero, ref uuidIExtractImage);
            }
        }
        catch { return null; }

    }

    //==========================================================
    //renvoie une instance de l'interface d'extraction d'icône pour item
    //==========================================================
    //IN item : instance de IShellFolder depuis laquelle on extrait l'extracteur d'icône
    //IN pidl : pointeur vers un ITEMLIST contenant le nom de fichier dont on veut l'icône
    //renvoie une instance de IExtractIcon
    //==========================================================
    private IExtractIcon getIcon(IShellFolder item, IntPtr pidl)
    {
        int prgf = 0;
        Guid uuidIExtractIcon = new Guid("000214FA-0000-0000-C000-000000000046");

        //idem
        //iid = "{000214fa-0000-0000-c000-000000000046}"

        try
        {
            if (pidl != IntPtr.Zero)
            {
                return (IExtractIcon)item.GetUIObjectOf(IntPtr.Zero, 1, ref pidl, ref uuidIExtractIcon, out prgf);
            }
            else
            {
                return (IExtractIcon)item.CreateViewObject(IntPtr.Zero, ref uuidIExtractIcon);
            }
        }
        catch { return null; }
    }

    //==========================================================
    //renvoie la miniature (ou à défaut l'icône) du fichier szFileName
    //=========================================================
    //IN szFileName : nom du fichier dont on veut la miniature
    //IN dwCX : largeur de la miniature
    //IN dwCY : hauteur de la miniature
    //renvoie une instance d'image VB IPictureDisp
    //==========================================================
    public System.Drawing.Bitmap ExtractImage(string szFileName, int dwCX, int dwCY)
    {
        int priority = 0;
        int requestedColourDepth;
        int flags;
        SIZE sz;
        IntPtr pidl = IntPtr.Zero;
        IShellFolder isf=null;
        IExtractImage ie=null;
        IExtractIcon ii=null;
        StringBuilder szPath;
        int pindex = 0;
        IntPtr pIconLarge = IntPtr.Zero;
        IntPtr pIconSmall = IntPtr.Zero;
        SHFILEINFO shgfi = new SHFILEINFO();
        System.Drawing.Bitmap ret = null;

        requestedColourDepth = 32;
        flags = (int)(EIEIFLAG.IEIFLAG_ASPECT | EIEIFLAG.IEIFLAG_OFFLINE | EIEIFLAG.IEIFLAG_SCREEN);

        //on récupère le nom de fichier sous forme ITEMLIST (pidl) et le dossier parent sous forme IShellFolder
        isf = GetShellFolder(szFileName, ref pidl);
        //on essaie de demander la miniature
        ie = getThumbnail(isf, pidl);
        //si pas possible
        if (ie == null)
        {
            //l'icône
            ii = getIcon(isf, pidl);

            //si possible
            if (!(ii == null))
            {
                //on extrait l'icône
                szPath = new StringBuilder(260);

                ii.GetIconLocation(0, szPath, 260, ref pindex, ref flags);
                ii.Extract(szPath.ToString(), pindex, ref pIconLarge, ref pIconSmall, dwCX + 65536 * dwCX);

                if (pIconLarge == IntPtr.Zero)
                {
                    SHGetFileInfo(szFileName, 0, ref shgfi, Marshal.SizeOf(shgfi), SHGFIFLAGS.SHGFI_LARGEICON | SHGFIFLAGS.SHGFI_ICON | SHGFIFLAGS.SHGFI_OVERLAYINDEX);
                    ret = Icon.FromHandle(shgfi.hIcon).ToBitmap();
                }
                else
                {
                    ret = Icon.FromHandle(pIconLarge).ToBitmap();
                }
            }
            else
            {
                SHGetFileInfo(szFileName, 0, ref shgfi, Marshal.SizeOf(shgfi), SHGFIFLAGS.SHGFI_LARGEICON | SHGFIFLAGS.SHGFI_ICON | SHGFIFLAGS.SHGFI_OVERLAYINDEX);
                ret = Icon.FromHandle(shgfi.hIcon).ToBitmap();
            }

            //si possible
        }
        else
        {
            //on extrait la miniature de la taille voulue
            sz.cx = dwCX;
            sz.cy = dwCY;

            szPath = new StringBuilder(260);
            EIEIFLAG f = (EIEIFLAG)flags;
            int r = ie.GetLocation(szPath, szPath.Capacity, out priority, ref sz, requestedColourDepth, ref f);
            try
            {
                pIconLarge = ie.Extract();
            }
            catch
            {
            }

            //si pas possible
            if (pIconLarge == IntPtr.Zero)
            {
                //on réessaie l'icône
                ii = getIcon(isf, pidl);

                //si possible
                if (!(ii == null))
                {
                    szPath = new StringBuilder(260);

                    ii.GetIconLocation(0, szPath, 260, ref pindex, ref flags);
                    ii.Extract(szPath.ToString(), pindex, ref pIconLarge, ref pIconSmall, dwCX + 65536 * dwCX);

                    if (pIconLarge == IntPtr.Zero)
                    {
                        SHGetFileInfo(szFileName, 0, ref shgfi, Marshal.SizeOf(shgfi), SHGFIFLAGS.SHGFI_LARGEICON | SHGFIFLAGS.SHGFI_ICON);
                        ret = Icon.FromHandle(shgfi.hIcon).ToBitmap();
                    }
                    else
                    {
                        ret = Icon.FromHandle(pIconLarge).ToBitmap();
                    }
                }
                else
                {
                    SHGetFileInfo(szFileName, 0, ref shgfi, Marshal.SizeOf(shgfi), SHGFIFLAGS.SHGFI_LARGEICON | SHGFIFLAGS.SHGFI_ICON);
                    ret = Icon.FromHandle(shgfi.hIcon).ToBitmap();
                }
            }
            else
            {
                ret = (Bitmap)Bitmap.FromHbitmap(pIconLarge);
            }
        }
        ILFree(pidl);
        if (ret == null)
        {
            ret.MakeTransparent();
        }
        if (pIconLarge != IntPtr.Zero) DeleteObject(pIconLarge);
        if (pIconSmall != IntPtr.Zero) DeleteObject(pIconSmall);
        if (isf != null) Marshal.ReleaseComObject(isf);
        if (ie != null) Marshal.ReleaseComObject(isf);
        if (ii != null) Marshal.ReleaseComObject(isf);
        return ret;
    }

    private Bitmap thumbBitmap;
    public Bitmap Thumbnail
    {
        get { return thumbBitmap; }
        set { thumbBitmap = value; }
    }

    public FileThumbnail(string szFileName, int cx, int cy)
    {
        if (string.IsNullOrEmpty(szFileName))
            throw new ArgumentNullException("szFileName");
        if (cx < 0)
            throw new ArgumentOutOfRangeException("cx");
        if (cy < 0)
            throw new ArgumentOutOfRangeException("cy");

        this.thumbBitmap = this.ExtractImage(szFileName, cx, cy);
    }

    #region IDisposable Members

    public void Dispose()
    {
        if (this.thumbBitmap != null)
        {
            this.thumbBitmap.Dispose();
            this.thumbBitmap = null;
        }
    }

    #endregion
}

