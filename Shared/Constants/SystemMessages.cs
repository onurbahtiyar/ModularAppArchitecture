public static class SystemMessages
{
    // Genel Başarı Mesajları
    public static string RegistrationSuccessful = "Kayıt işlemi başarıyla tamamlandı.";

    public static string OperationSuccessful = "İşlem başarıyla gerçekleştirildi.";
    public static string RequestSubmittedSuccessfully = "Talebiniz başarıyla alındı.";

    // Genel Hata Mesajları
    public static string UserNotFound = "Kullanıcı bulunamadı.";

    public static string RecordNotFound = "İstenen kayıt bulunamadı.";
    public static string UnknownError = "Bilinmeyen bir hata meydana geldi.";
    public static string OperationFailed = "İşlem sırasında bir hata meydana geldi. Lütfen tekrar deneyin.";
    public static string ErrorFetchingData = "Veri getirilirken bir hata oluştu. Lütfen daha sonra tekrar deneyin.";

    // Yetkilendirme Mesajları
    public static string AccessDenied = "Bu işlemi gerçekleştirmek için yetkiniz bulunmamaktadır.";

    public static string AccessGranted = "Bu işlemi gerçekleştirmek için gerekli yetkiye sahipsiniz.";
    public static string UnauthorizedAccessAttempt = "Yetkisiz erişim girişimi tespit edildi.";
    public static string UserAuthorizationFailed = "Kullanıcı yetkilendirme işlemi başarısız oldu.";
    public static string InvalidRequestStatusForOperation = "Talebin mevcut durumu, bu işlemi gerçekleştirmek için uygun değil.";

    // Tekrar Eden ve Limit Aşımı Mesajları
    public static string DuplicateOperation = "Bu işlem zaten gerçekleştirilmiş. Lütfen mevcut durumu kontrol edin.";

    public static string RateLimitExceeded = "İstek limitinizi aştınız. Lütfen daha sonra tekrar deneyin.";
    public static string ResourceAlreadyExists = "Bu kayıt zaten mevcut.";

    // Doğrulama ve Giriş Mesajları
    public static string InvalidVerificationCode = "Geçersiz doğrulama kodu. Lütfen doğru kodu girin veya destek ekibimizle iletişime geçin.";

    public static string InvalidCredentials = "Kullanıcı adı veya şifre hatalı. Lütfen bilgilerinizi kontrol edin ve tekrar deneyin.";
    public static string AccountLocked = "Hesabınız çok fazla hatalı giriş nedeniyle kilitlendi. Lütfen bir süre sonra tekrar deneyin veya destek ekibiyle iletişime geçin.";
    public static string PasswordResetRequired = "Güvenlik nedeniyle şifrenizin yenilenmesi gerekiyor. Lütfen şifre sıfırlama işlemini gerçekleştirin.";

    // Veri ve İçerik Doğrulama Mesajları
    public static string ContentLengthMismatch = "Gönderilen verinin boyutu beklenenden farklı. Lütfen gönderilen veriyi kontrol edin.";

    public static string MissingRequiredFields = "Gerekli alanlar eksik. Lütfen tüm zorunlu bilgileri doldurun ve tekrar deneyin.";
    public static string InvalidDataFormat = "Gönderilen veri formatı geçersiz. Lütfen uygun formatta veri gönderin.";

    // Oturum ve Bağlantı Mesajları
    public static string SessionExpired = "Oturumunuzun süresi doldu. Lütfen yeniden giriş yapın.";

    public static string ConnectionFailed = "Sunucu ile bağlantı kurulamadı. Lütfen internet bağlantınızı kontrol edin ve tekrar deneyin.";
    public static string DisconnectedFromServer = "Sunucu bağlantısı kesildi. Lütfen tekrar bağlanmayı deneyin.";

    // Uygulama ve Sistem Mesajları
    public static string SystemMaintenance = "Sistem bakımda. Lütfen daha sonra tekrar deneyin.";

    public static string FeatureNotAvailable = "Bu özellik şu anda kullanılamıyor. Lütfen daha sonra tekrar deneyin.";
    public static string UpdateRequired = "Devam edebilmek için uygulamayı güncellemeniz gerekiyor. Lütfen en son sürümü indirin.";
    public static string OperationTimedOut = "İşlem zaman aşımına uğradı. Lütfen tekrar deneyin.";
}