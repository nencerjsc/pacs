namespace NencerApi.Modules.User.Model
{
    #region Class
    public class PermissionDefined
    {
        #region ĐÓN TIẾP
        public class MENU_DONTIEPNHANH
        {
            [Description("ĐÓN TIẾP")]
            public const string PREFIX = "MENU_DONTIEPNHANH";
            [Description("Hiện Menu")]
            public const string HIENMENU = PREFIX + ".HIENMENU";

            [Description("Đón tiếp")]
            public const string DONTIEP = "DONTIEP";
            [Description("Hẹn khám")]
            public const string HENKHAM = "HENKHAM";
            [Description("Đặt lịch")]
            public const string DATLICH = "DATLICH";
        }

        public class MENU_DONTIEP
        {
            [Description("ĐÓN TIẾP")]
            public const string PREFIX = "MENU_DONTIEP";
            [Description("Hiện Menu")]
            public const string HIENMENU = PREFIX + ".HIENMENU";

            [Description("Đón tiếp")]
            public const string DONTIEP = "DONTIEP";
            [Description("Hẹn khám")]
            public const string HENKHAM = "HENKHAM";
            [Description("Đặt lịch")]
            public const string DATLICH = "DATLICH";
        }

        public class DONTIEP
        {
            [Description("Đón tiếp")]
            public const string PREFIX = "DONTIEP";
            [Description("Hiện Menu")]
            public const string HIENMENU = PREFIX + ".HIENMENU";

            [Description("Tạo đón tiếp")]
            public const string TAODONTIEP = PREFIX + ".TAODONTIEP";
            [Description("Sửa đón tiếp")]
            public const string SUADONTIEP = PREFIX + ".SUADONTIEP";
            [Description("Xóa đón tiếp")]
            public const string XOADONTIEP = PREFIX + ".XOADONTIEP";
            [Description("Đổi phòng khám")]
            public const string DOIPHONGKHAM = PREFIX + ".DOIPHONGKHAM";

        }

        public class HENKHAM
        {
            [Description("Hẹn khám")]
            public const string PREFIX = "HENKHAM";
            [Description("Hiện Menu")]
            public const string HIENMENU = PREFIX + ".HIENMENU";


        }

        public class DATLICH
        {
            [Description("Đặt lịch")]
            public const string PREFIX = "DATLICH";
            [Description("Hiện Menu")]
            public const string HIENMENU = PREFIX + ".HIENMENU";
        }
        #endregion ĐÓN TIẾP

        #region KHÁM BỆNH
        public class MENU_KHAMBENH
        {
            [Description("KHÁM BỆNH")]
            public const string PREFIX = "MENU_KHAMBENH";
            [Description("Hiện Menu")]
            public const string HIENMENU = PREFIX + ".HIENMENU";

            [Description("Bắt đầu khám")]
            public const string BATDAUKHAM = PREFIX + ".BATDAUKHAM";
            [Description("Hoàn thành khám")]
            public const string HOANTHANHKHAM = PREFIX + ".HOANTHANHKHAM";
            [Description("Bỏ khám")]
            public const string BOKHAM = PREFIX + ".BOKHAM";

            [Description("Lưu kết quả khám bệnh")]
            public const string LUUKETQUAKHAMBENH = PREFIX + ".LUUKETQUAKHAMBENH";

            [Description("Lưu kết luận tổng hợp")]
            public const string LUUKETLUANTONGHOP = PREFIX + ".LUUKETLUANTONGHOP";

            [Description("Chỉ định thuốc - vật tư")]
            public const string THEMTOATHUOC = PREFIX + ".THEMTOATHUOC";
            [Description("Sửa toa thuốc")]
            public const string SUATOATHUOC = PREFIX + ".SUATOATHUOC";
            [Description("Xóa toa thuốc")]
            public const string XOATOATHUOC = PREFIX + ".XOATOATHUOC";

            [Description("Chỉ định dịch vụ")]
            public const string CHIDINHDICHVU = PREFIX + ".TAOCHIDINH";
            [Description("Sửa chỉ định dịch vụ")]
            public const string SUACHIDINH = PREFIX + ".SUACHIDINH";
            [Description("Xóa chỉ định dịch vụ")]
            public const string XOACHIDINH = PREFIX + ".XOACHIDINH";

            [Description("Tạo xử trí")]
            public const string TAOXUTRI = PREFIX + ".TAOXUTRI";
            [Description("Mở lại bệnh án")]
            public const string MOLAIBENHAN = PREFIX + ".MOLAIBENHAN";
        }
        #endregion KHÁM BỆNH


        #region XÉT NGHIỆM
        ///menu xét nghiệm
        public class MENU_LIS
        {
            [Description("LIS")]
            public const string PREFIX = "MENU_LIS";
            [Description("Hiện Menu")]
            public const string HIENMENU = PREFIX + ".HIENMENU";

            [Description("Lấy mẫu")]
            public const string LAYMAU = "LAYMAU";
            [Description("Nhận mẫu")]
            public const string NHANMAU = "NHANMAU";
            [Description("Xét nghiệm")]
            public const string XETNGHIEM = "XETNGHIEM";
        }
        public class LAYMAU
        {
            [Description("Lấy mẫu")]
            public const string PREFIX = "LAYMAU";
            [Description("Hiện Menu")]
            public const string HIENMENU = PREFIX + ".HIENMENU";

            [Description("Tạo lấy mẫu")]
            public const string TAOLAYMAU = PREFIX + "TAOLAYMAU";
            [Description("Sửa lấy mẫu")]
            public const string SUANHANMAU = PREFIX + "SUANHANMAU";
            [Description("Hủy lấy mẫu")]
            public const string HUYNHANMAU = PREFIX + "HUYNHANMAU";
        }
        public class NHANMAU
        {
            [Description("Nhận mẫu")]
            public const string PREFIX = "NHANMAU";
            [Description("Hiện Menu")]
            public const string HIENMENU = PREFIX + ".HIENMENU";

            [Description("Nhận mẫu")]
            public const string TAONHANMAU = PREFIX + "TAONHANMAU";
            [Description("Sửa nhận mẫu")]
            public const string SUANHANMAU = PREFIX + "SUANHANMAU";
            [Description("Hủy nhận mẫu")]
            public const string HUYNHANMAU = PREFIX + "HUYNHANMAU";
        }
        public class XETNGHIEM
        {
            [Description("Xét nghiệm")]
            public const string PREFIX = "XETNGHIEM";
            [Description("Hiện Menu")]
            public const string HIENMENU = PREFIX + ".HIENMENU";

            [Description("Sửa kết quả trên view")]
            public const string SUAKETQUA = PREFIX + "SUAKETQUA";

            [Description("Duyệt kết quả")]
            public const string DUYETKETQUA = PREFIX + "DUYETKETQUA";
            [Description("Hủy duyệt kết quả")]
            public const string HUYDUYETKETQUA = PREFIX + "HUYDUYETKETQUA";

            [Description("Trả kết quả")]
            public const string TRAKETQUA = PREFIX + "TRAKETQUA";
            [Description("Hủy trả kết quả")]
            public const string HUYTRAKETQUA = PREFIX + "HUYTRAKETQUA";

            [Description("Sửa ghi chú")]
            public const string SUAGHICHU = PREFIX + "SUAGHICHU";

            [Description("Xuất file kết quả")]
            public const string XUATFILEKETQUA = PREFIX + "XUATFILEKETQUA";
            [Description("Xuất file kết quả tùy chọn")]
            public const string XUATFILEKETQUATUYCHON = PREFIX + "XUATFILEKETQUATUYCHON";
        }
        #endregion

        #region RIS - CHẨN ĐOÁN HÌNH ẢNH
        public class MENU_RIS
        {
            [Description("Chuẩn đoán hình ảnh")]
            public const string PREFIX = "MENU_RIS";
            [Description("Hiện Menu")]
            public const string HIENMENU = PREFIX + ".HIENMENU";

            [Description("Thêm mẫu trả KQ")]
            public const string THEMMAU = PREFIX + ".THEMMAU";
            [Description("Sửa mẫu trả KQ")]
            public const string SUAMAU = PREFIX + ".SUAMAU";

            [Description("Thực hiện")]
            public const string THUCHIEN = PREFIX + ".THUCHIEN";
            [Description("Hủy thực hiện")]
            public const string HUYTHUCHIEN = PREFIX + ".HUYTHUCHIEN";
            [Description("Lưu kết quả")]
            public const string LUUKETQUA = PREFIX + ".LUUKETQUA";
            [Description("Trả kết quả")]
            public const string TRAKETQUA = PREFIX + ".TRAKETQUA";
            [Description("Hủy trả KQ")]
            public const string HUYTRAKETQUA = PREFIX + ".HUYTRAKETQUA";

        }
        #endregion RIS - CHẨN ĐOÁN HÌNH ẢNH

        #region PTTT
        public class MENU_PHAUTHUATTHUTHUAT
        {
            [Description("Phẫu thuật thủ thuật")]
            public const string PREFIX = "MENU_PHAUTHUATTHUTHUAT";
            [Description("Hiện Menu")]
            public const string HIENMENU = PREFIX + ".HIENMENU";
        }
        #endregion PTTT

        #region NHÀ THUỐC
        ///menu nhà thuốc
        public class MENU_NHATHUOC
        {
            [Description("Nhà thuốc")]
            public const string PREFIX = "MENU_NHATHUOC";
            [Description("Hiện Menu")]
            public const string HIENMENU = PREFIX + ".HIENMENU";

            [Description("Duyệt toa thuốc")]
            public const string DUYETTOATHUOC = "DUYETTOATHUOC";
            [Description("Thanh toán thuốc")]
            public const string THANHTOANTHUOC = "THANHTOANTHUOC";
            [Description("Cấp phát thuốc")]
            public const string CAPPHATTHUOC = "CAPPHATTHUOC";
            [Description("Quản lý kho")]
            public const string QUANLYKHO = "QUANLYKHO";
        }
        public class DUYETTOATHUOC
        {
            [Description("Duyệt toa thuốc")]
            public const string PREFIX = "DUYETTOATHUOC";
            [Description("Hiện Menu")]
            public const string HIENMENU = PREFIX + ".HIENMENU";

            [Description("Thêm toa bán lẻ")]
            public const string THEMTOABANLE = PREFIX + ".THEMTOABANLE";
            [Description("Điều chỉnh toa")]
            public const string DIEUCHINHTOA = PREFIX + ".DIEUCHINHTOA";
            [Description("Duyệt toa")]
            public const string DUYETTOA = PREFIX + ".DUYETTOA";
            [Description("Hủy duyệt toa")]
            public const string HUYDUYETTOA = PREFIX + ".HUYDUYETTOA";
        }
        public class THANHTOANTHUOC
        {
            [Description("Thanh toán thuốc")]
            public const string PREFIX = "THANHTOANTHUOC";
            [Description("Hiện Menu")]
            public const string HIENMENU = PREFIX + ".HIENMENU";

            [Description("Thanh toán thuốc")]
            public const string TAOTHANHTOANTHUOC = PREFIX + ".TAOTHANHTOANTHUOC";
            [Description("Hủy thanh toán thuốc")]
            public const string HUYTHANHTOANTHUOC = PREFIX + ".HUYTHANHTOANTHUOC";
        }
        public class CAPPHATTHUOC
        {
            [Description("Cấp phát thuốc")]
            public const string PREFIX = "CAPPHATTHUOC";
            [Description("Hiện Menu")]
            public const string HIENMENU = PREFIX + ".HIENMENU";

            [Description("Thanh toán thuốc")]
            public const string CAPPHAT = PREFIX + ".CAPPHAT";
            [Description("Hủy thanh toán thuốc")]
            public const string HUYCAPPHAT = PREFIX + ".HUYCAPPHAT";
        }
        public class QUANLYKHO
        {
            [Description("Quản lý kho")]
            public const string PREFIX = "QUANLYKHO";
            [Description("Hiện Menu")]
            public const string HIENMENU = PREFIX + ".HIENMENU";
        }
        #endregion

        #region QUẢN LÝ KHO
        ///menu kho thuốc
        public class MENU_QUANLYKHO
        {
            [Description("Kho thuốc")]
            public const string PREFIX = "MENU_QUANLYKHO";
            [Description("Hiện Menu")]
            public const string HIENMENU = PREFIX + ".HIENMENU";

            [Description("Danh mục thuốc")]
            public const string DANHMUCTHUOC = "DANHMUCTHUOC";
            [Description("Nhà cung cấp")]
            public const string NHACUNGCAP = "NHACUNGCAP";
            [Description("Danh sách kho")]
            public const string DANHSACHKHO = "DANHSACHKHO";
            [Description("Sản phẩm thuốc")]
            public const string SANPHAMTHUOC = "SANPHAMTHUOC";
            [Description("Gói thầu")]
            public const string GOITHAU = "GOITHAU";
            [Description("Nhập thuốc")]
            public const string NHAPKHO = "NHAPKHO";
            [Description("Xuất kho")]
            public const string XUATKHO = "XUATKHO";
            [Description("Chuyển kho")]
            public const string CHUYENKHO = "CHUYENKHO";
            [Description("Tồn kho")]
            public const string TONKHO = "TONKHO";
        }
        public class DANHMUCTHUOC
        {
            [Description("Danh mục thuốc")]
            public const string PREFIX = "DANHMUCTHUOC";
            [Description("Hiện Menu")]
            public const string HIENMENU = PREFIX + ".HIENMENU";
        }
        public class NHACUNGCAP
        {
            [Description("Nhà cung cấp")]
            public const string PREFIX = "NHACUNGCAP";
            [Description("Hiện Menu")]
            public const string HIENMENU = PREFIX + ".HIENMENU";
        }
        public class DANHSACHKHO
        {
            [Description("Danh sách kho")]
            public const string PREFIX = "DANHSACHKHO";
            [Description("Hiện Menu")]
            public const string HIENMENU = PREFIX + ".HIENMENU";
        }
        public class SANPHAMTHUOC
        {
            [Description("Sản phẩm thuốc")]
            public const string PREFIX = "SANPHAMTHUOC";
            [Description("Hiện Menu")]
            public const string HIENMENU = PREFIX + ".HIENMENU";
        }
        public class GOITHAU
        {
            [Description("Gói thầu")]
            public const string PREFIX = "GOITHAU";
            [Description("Hiện Menu")]
            public const string HIENMENU = PREFIX + ".HIENMENU";
        }

        public class NHAPKHO
        {
            [Description("Nhập kho")]
            public const string PREFIX = "NHAPKHO";
            [Description("Hiện Menu")]
            public const string HIENMENU = PREFIX + ".HIENMENU";

            [Description("Thêm phiếu nhập kho")]
            public const string TAOPHIEUNHAPKHO = PREFIX + ".TAOPHIEUNHAPKHO";
            [Description("Sửa phiếu nhập kho")]
            public const string SUAPHIEUNHAPKHO = PREFIX + ".SUAPHIEUNHAPKHO";
            [Description("Duyệt nhập kho")]
            public const string DUYETNHAPKHO = PREFIX + ".DUYETNHAPKHO";
            [Description("Hủy nhập kho")]
            public const string HUYNHAPKHO = PREFIX + ".HUYNHAPKHO";
            [Description("Xóa phiếu nhập kho")]
            public const string XOAPHIEUNHAPKHO = PREFIX + ".XOAPHIEUNHAPKHO";
        }
        public class XUATKHO
        {
            [Description("Xuất kho")]
            public const string PREFIX = "XUATKHO";
            [Description("Hiện Menu")]
            public const string HIENMENU = PREFIX + ".HIENMENU";

            [Description("Thêm phiếu xuất kho")]
            public const string TAOPHIEUXUATKHO = PREFIX + ".TAOPHIEUXUATKHO";
            [Description("Sửa phiếu xuất kho")]
            public const string SUAPHIEUXUATKHO = PREFIX + ".SUAPHIEUXUATKHO";
            [Description("Duyệt xuất kho")]
            public const string DUYETXUATKHO = PREFIX + ".DUYETXUATKHO";
            [Description("Hủy duyệt xuất kho")]
            public const string HUYDUYETXUATKHO = PREFIX + ".HUYDUYETXUATKHO";
            [Description("Xóa phiếu xuất kho")]
            public const string XOAPHIEUXUATKHO = PREFIX + ".XOAPHIEUXUATKHO";
        }
        public class CHUYENKHO
        {
            [Description("Chuyển kho")]
            public const string PREFIX = "CHUYENKHO";
            [Description("Hiện Menu")]
            public const string HIENMENU = PREFIX + ".HIENMENU";
        }
        public class TONKHO
        {
            [Description("Tồn kho")]
            public const string PREFIX = "TONKHO";
            [Description("Hiện Menu")]
            public const string HIENMENU = PREFIX + ".HIENMENU";
        }
        #endregion

        #region BỆNH NHÂN
        /// menu bệnh nhân
        public class MENU_BENHNHAN
        {
            [Description("Bệnh nhân")]
            public const string PREFIX = "MENU_BENHNHAN";
            [Description("Hiện Menu")]
            public const string HIENMENU = PREFIX + ".HIENMENU";

            [Description("Bệnh nhân")]
            public const string BENHNHAN = "BENHNHAN";

        }
        public class BENHNHAN
        {
            [Description("Bệnh nhân")]
            public const string PREFIX = "BENHNHAN";
            [Description("Hiện Menu")]
            public const string HIENMENU = PREFIX + ".HIENMENU";
        }
        #endregion

        #region BÁO CÁO
        /// <summary>
        /// Báo cáo
        /// </summary>
        public class MENU_BAOCAO
        {
            [Description("Menu báo cáo")]
            public const string PREFIX = "MENU_BAOCAO";
            [Description("Hiện Menu")]
            public const string HIENMENU = PREFIX + ".HIENMENU";

            [Description("Báo cáo")]
            public const string BAOCAO = "BAOCAO";
        }
        #endregion

        #region DANH MỤC
        public class MENU_DANHMUC
        {
            [Description("Danh mục")]
            public const string PREFIX = "MENU_DANHMUC";
            [Description("Hiện Menu")]
            public const string HIENMENU = PREFIX + ".HIENMENU";

            [Description("Dsanh sách khoa")]
            public const string DANHSACHKHOA = "DANHSACHKHOA";
            [Description("Dsanh sách phòng")]
            public const string DANHSACHPHONG = "DANHSACHPHONG";

            [Description("Dịch vụ")]
            public const string DICHVU = "DICHVU";
            [Description("Sản phẩm")]
            public const string SANPHAM = "SANPHAM";
            [Description("Nhóm dịch vụ")]
            public const string NHOMSANPHAM = "NHOMSANPHAM";
            [Description("Loại dịch vụ")]
            public const string DANHMUCSANPHAM = "DANHMUCSANPHAM";

            [Description("Cổng thanh toán")]
            public const string CONGTHANHTOAN = "CONGTHANHTOAN";
            [Description("Mẫu báo cáo")]
            public const string MAUBAOCAO = "MAUBAOCAO";

            [Description("Hợp đồng khám đoàn")]
            public const string HOPDONGKHAMDOAN = "HOPDONGKHAMDOAN";
            [Description("Bệnh nhân")]
            public const string BENHNHAN = "BENHNHAN";
            [Description("Loại bệnh nhân")]
            public const string LOAIBENHNHAN = "LOAIBENHNHAN";

            [Description("Đơn vị tính")]
            public const string DONVITINH = "DONVITINH";
            [Description("Đường dùng")]
            public const string DUONGDUNG = "DUONGDUNG";
            [Description("Hoạt chất")]
            public const string HOATCHAT = "HOATCHAT";
        }

        public class DANHSACHKHOA
        {
            [Description("Danh sách khoa")]
            public const string PREFIX = "DANHSACHKHOA";
            [Description("Hiện Menu")]
            public const string HIENMENU = PREFIX + ".HIENMENU";
        }
        public class DANHSACHPHONG
        {
            [Description("Danh sách phòng")]
            public const string PREFIX = "DANHSACHPHONG";
            [Description("Hiện Menu")]
            public const string HIENMENU = PREFIX + ".HIENMENU";
        }

        public class DICHVU
        {
            [Description("Dịch vụ")]
            public const string PREFIX = "DICHVU";
            [Description("Hiện Menu")]
            public const string HIENMENU = PREFIX + ".HIENMENU";
        }
        public class SANPHAM
        {
            [Description("Sản phẩm")]
            public const string PREFIX = "SANPHAM";
            [Description("Hiện Menu")]
            public const string HIENMENU = PREFIX + ".HIENMENU";
        }
        public class NHOMSANPHAM
        {
            [Description("Nhóm sản phẩm")]
            public const string PREFIX = "NHOMSANPHAM";
            [Description("Hiện Menu")]
            public const string HIENMENU = PREFIX + ".HIENMENU";
        }
        public class DANHMUCSANPHAM
        {
            [Description("Danh mục sản phẩm")]
            public const string PREFIX = "DANHMUCSANPHAM";
            [Description("Hiện Menu")]
            public const string HIENMENU = PREFIX + ".HIENMENU";
        }
        public class CONGTHANHTOAN
        {
            [Description("Cổng thanh toán")]
            public const string PREFIX = "CONGTHANHTOAN";
            [Description("Hiện Menu")]
            public const string HIENMENU = PREFIX + ".HIENMENU";
        }
        public class MAUBAOCAO
        {
            [Description("Mẫu báo cáo")]
            public const string PREFIX = "MAUBAOCAO";
            [Description("Hiện Menu")]
            public const string HIENMENU = PREFIX + ".HIENMENU";
        }
        public class HOPDONGKHAMDOAN
        {
            [Description("Hợp đồng khám đoàn")]
            public const string PREFIX = "HOPDONGKHAMDOAN";
            [Description("Hiện Menu")]
            public const string HIENMENU = PREFIX + ".HIENMENU";
        }

        public class DONVITINH
        {
            [Description("Đơn vị tính")]
            public const string PREFIX = "DONVITINH";
            [Description("Hiện Menu")]
            public const string HIENMENU = PREFIX + ".HIENMENU";
        }
        public class DUONGDUNG
        {
            [Description("Đường dùng")]
            public const string PREFIX = "DUONGDUNG";
            [Description("Hiện Menu")]
            public const string HIENMENU = PREFIX + ".HIENMENU";
        }
        public class HOATCHAT
        {
            [Description("Hoạt chất")]
            public const string PREFIX = "HOATCHAT";
            [Description("Hiện Menu")]
            public const string HIENMENU = PREFIX + ".HIENMENU";
        }
        #endregion

        #region KẾ TOÁN
        /// <summary>
        /// Khối Kế toán
        /// </summary>
        public class MENU_THUNGAN
        {
            [Description("Thu ngân")]
            public const string PREFIX = "MENU_THUNGAN";
            [Description("Hiện Menu")]
            public const string HIENMENU = PREFIX + ".HIENMENU";

            [Description("Tạo hóa đơn")]
            public const string TAOHOADON = PREFIX + ".TAOHOADON";
            [Description("Sửa hóa đơn")]
            public const string SUAHOADON = PREFIX + ".SUAHOADON";
            [Description("Hủy hóa đơn")]
            public const string HUYHOADON = PREFIX + ".HUYHOADON";
        }
        #endregion

        #region HỆ THỐNG
        ///menu he thong
        public class MENU_HETHONG
        {
            [Description("Hệ thống")]
            public const string PREFIX = "MENU_HETHONG";
            [Description("Hiện Menu")]
            public const string HIENMENU = PREFIX + ".HIENMENU";

            [Description("Tài khoản")]
            public const string NHANSU = "NHANSU";
            [Description("Phân quyền")]
            public const string PHANQUYEN = "PHANQUYEN";
            [Description("Cấu hình")]
            public const string CAUHINH = "CAUHINH";
        }

        public class NHANSU
        {
            [Description("Nhân sự")]
            public const string PREFIX = "NHANSU";
            [Description("Hiện Menu")]
            public const string HIENMENU = PREFIX + ".HIENMENU";
        }
        public class PHANQUYEN
        {
            [Description("Phân quyền")]
            public const string PREFIX = "PHANQUYEN";
            [Description("Hiện Menu")]
            public const string HIENMENU = PREFIX + ".HIENMENU";
        }
        public class CAUHINH
        {
            [Description("Cấu hình")]
            public const string PREFIX = "CAUHINH";
            [Description("Hiện Menu")]
            public const string HIENMENU = PREFIX + ".HIENMENU";
        }
        #endregion
    }
    #endregion
}
