using System.Runtime.InteropServices;

namespace NullOpsDevs.LibSsh.Generated;

public static unsafe partial class LibSshNative
{
    [DllImport("libssh2", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
    public static extern int libssh2_sign_sk([NativeTypeName("LIBSSH2_SESSION *")] _LIBSSH2_SESSION* session, [NativeTypeName("unsigned char **")] byte** sig, [NativeTypeName("size_t *")] nuint* sig_len, [NativeTypeName("const unsigned char *")] byte* data, [NativeTypeName("size_t")] nuint data_len, void** @abstract);

    [DllImport("libssh2", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
    public static extern int libssh2_init(int flags);

    [DllImport("libssh2", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
    public static extern void libssh2_exit();

    [DllImport("libssh2", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
    public static extern void libssh2_free([NativeTypeName("LIBSSH2_SESSION *")] _LIBSSH2_SESSION* session, void* ptr);

    [DllImport("libssh2", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
    public static extern int libssh2_session_supported_algs([NativeTypeName("LIBSSH2_SESSION *")] _LIBSSH2_SESSION* session, int method_type, [NativeTypeName("const char ***")] sbyte*** algs);

    [DllImport("libssh2", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
    [return: NativeTypeName("LIBSSH2_SESSION *")]
    public static extern _LIBSSH2_SESSION* libssh2_session_init_ex([NativeTypeName("void *(*)(size_t, void **)")] delegate* unmanaged[Cdecl]<nuint, void**, void*> my_alloc, [NativeTypeName("void (*)(void *, void **)")] delegate* unmanaged[Cdecl]<void*, void**, void> my_free, [NativeTypeName("void *(*)(void *, size_t, void **)")] delegate* unmanaged[Cdecl]<void*, nuint, void**, void*> my_realloc, void* @abstract);

    [DllImport("libssh2", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
    public static extern void** libssh2_session_abstract([NativeTypeName("LIBSSH2_SESSION *")] _LIBSSH2_SESSION* session);

    [DllImport("libssh2", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
    [return: NativeTypeName("libssh2_cb_generic *")]
    public static extern delegate* unmanaged[Cdecl]<void> libssh2_session_callback_set2([NativeTypeName("LIBSSH2_SESSION *")] _LIBSSH2_SESSION* session, int cbtype, [NativeTypeName("libssh2_cb_generic *")] delegate* unmanaged[Cdecl]<void> callback);

    [DllImport("libssh2", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
    [Obsolete("Use libssh2_session_callback_set2()")]
    public static extern void* libssh2_session_callback_set([NativeTypeName("LIBSSH2_SESSION *")] _LIBSSH2_SESSION* session, int cbtype, void* callback);

    [DllImport("libssh2", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
    public static extern int libssh2_session_banner_set([NativeTypeName("LIBSSH2_SESSION *")] _LIBSSH2_SESSION* session, [NativeTypeName("const char *")] sbyte* banner);

    [DllImport("libssh2", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
    [Obsolete("Use libssh2_session_banner_set()")]
    public static extern int libssh2_banner_set([NativeTypeName("LIBSSH2_SESSION *")] _LIBSSH2_SESSION* session, [NativeTypeName("const char *")] sbyte* banner);

    [DllImport("libssh2", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
    [Obsolete("Use libssh2_session_handshake()")]
    public static extern int libssh2_session_startup([NativeTypeName("LIBSSH2_SESSION *")] _LIBSSH2_SESSION* session, int sock);

    [DllImport("libssh2", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
    public static extern int libssh2_session_handshake([NativeTypeName("LIBSSH2_SESSION *")] _LIBSSH2_SESSION* session, [NativeTypeName("libssh2_socket_t")] ulong sock);

    [DllImport("libssh2", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
    public static extern int libssh2_session_disconnect_ex([NativeTypeName("LIBSSH2_SESSION *")] _LIBSSH2_SESSION* session, int reason, [NativeTypeName("const char *")] sbyte* description, [NativeTypeName("const char *")] sbyte* lang);

    [DllImport("libssh2", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
    public static extern int libssh2_session_free([NativeTypeName("LIBSSH2_SESSION *")] _LIBSSH2_SESSION* session);

    [DllImport("libssh2", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
    [return: NativeTypeName("const char *")]
    public static extern sbyte* libssh2_hostkey_hash([NativeTypeName("LIBSSH2_SESSION *")] _LIBSSH2_SESSION* session, int hash_type);

    [DllImport("libssh2", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
    [return: NativeTypeName("const char *")]
    public static extern sbyte* libssh2_session_hostkey([NativeTypeName("LIBSSH2_SESSION *")] _LIBSSH2_SESSION* session, [NativeTypeName("size_t *")] nuint* len, int* type);

    [DllImport("libssh2", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
    public static extern int libssh2_session_method_pref([NativeTypeName("LIBSSH2_SESSION *")] _LIBSSH2_SESSION* session, int method_type, [NativeTypeName("const char *")] sbyte* prefs);

    [DllImport("libssh2", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
    [return: NativeTypeName("const char *")]
    public static extern sbyte* libssh2_session_methods([NativeTypeName("LIBSSH2_SESSION *")] _LIBSSH2_SESSION* session, int method_type);

    [DllImport("libssh2", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
    public static extern int libssh2_session_last_error([NativeTypeName("LIBSSH2_SESSION *")] _LIBSSH2_SESSION* session, [NativeTypeName("char **")] sbyte** errmsg, int* errmsg_len, int want_buf);

    [DllImport("libssh2", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
    public static extern int libssh2_session_last_errno([NativeTypeName("LIBSSH2_SESSION *")] _LIBSSH2_SESSION* session);

    [DllImport("libssh2", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
    public static extern int libssh2_session_set_last_error([NativeTypeName("LIBSSH2_SESSION *")] _LIBSSH2_SESSION* session, int errcode, [NativeTypeName("const char *")] sbyte* errmsg);

    [DllImport("libssh2", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
    public static extern int libssh2_session_block_directions([NativeTypeName("LIBSSH2_SESSION *")] _LIBSSH2_SESSION* session);

    [DllImport("libssh2", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
    public static extern int libssh2_session_flag([NativeTypeName("LIBSSH2_SESSION *")] _LIBSSH2_SESSION* session, int flag, int value);

    [DllImport("libssh2", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
    [return: NativeTypeName("const char *")]
    public static extern sbyte* libssh2_session_banner_get([NativeTypeName("LIBSSH2_SESSION *")] _LIBSSH2_SESSION* session);

    [DllImport("libssh2", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
    [return: NativeTypeName("char *")]
    public static extern sbyte* libssh2_userauth_list([NativeTypeName("LIBSSH2_SESSION *")] _LIBSSH2_SESSION* session, [NativeTypeName("const char *")] sbyte* username, [NativeTypeName("unsigned int")] uint username_len);

    [DllImport("libssh2", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
    public static extern int libssh2_userauth_banner([NativeTypeName("LIBSSH2_SESSION *")] _LIBSSH2_SESSION* session, [NativeTypeName("char **")] sbyte** banner);

    [DllImport("libssh2", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
    public static extern int libssh2_userauth_authenticated([NativeTypeName("LIBSSH2_SESSION *")] _LIBSSH2_SESSION* session);

    [DllImport("libssh2", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
    public static extern int libssh2_userauth_password_ex([NativeTypeName("LIBSSH2_SESSION *")] _LIBSSH2_SESSION* session, [NativeTypeName("const char *")] sbyte* username, [NativeTypeName("unsigned int")] uint username_len, [NativeTypeName("const char *")] sbyte* password, [NativeTypeName("unsigned int")] uint password_len, [NativeTypeName("void (*)(LIBSSH2_SESSION *, char **, int *, void **)")] delegate* unmanaged[Cdecl]<_LIBSSH2_SESSION*, sbyte**, int*, void**, void> passwd_change_cb);

    [DllImport("libssh2", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
    public static extern int libssh2_userauth_publickey_fromfile_ex([NativeTypeName("LIBSSH2_SESSION *")] _LIBSSH2_SESSION* session, [NativeTypeName("const char *")] sbyte* username, [NativeTypeName("unsigned int")] uint username_len, [NativeTypeName("const char *")] sbyte* publickey, [NativeTypeName("const char *")] sbyte* privatekey, [NativeTypeName("const char *")] sbyte* passphrase);

    [DllImport("libssh2", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
    public static extern int libssh2_userauth_publickey([NativeTypeName("LIBSSH2_SESSION *")] _LIBSSH2_SESSION* session, [NativeTypeName("const char *")] sbyte* username, [NativeTypeName("const unsigned char *")] byte* pubkeydata, [NativeTypeName("size_t")] nuint pubkeydata_len, [NativeTypeName("int (*)(LIBSSH2_SESSION *, unsigned char **, size_t *, const unsigned char *, size_t, void **)")] delegate* unmanaged[Cdecl]<_LIBSSH2_SESSION*, byte**, nuint*, byte*, nuint, void**, int> sign_callback, void** @abstract);

    [DllImport("libssh2", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
    public static extern int libssh2_userauth_hostbased_fromfile_ex([NativeTypeName("LIBSSH2_SESSION *")] _LIBSSH2_SESSION* session, [NativeTypeName("const char *")] sbyte* username, [NativeTypeName("unsigned int")] uint username_len, [NativeTypeName("const char *")] sbyte* publickey, [NativeTypeName("const char *")] sbyte* privatekey, [NativeTypeName("const char *")] sbyte* passphrase, [NativeTypeName("const char *")] sbyte* hostname, [NativeTypeName("unsigned int")] uint hostname_len, [NativeTypeName("const char *")] sbyte* local_username, [NativeTypeName("unsigned int")] uint local_username_len);

    [DllImport("libssh2", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
    public static extern int libssh2_userauth_publickey_frommemory([NativeTypeName("LIBSSH2_SESSION *")] _LIBSSH2_SESSION* session, [NativeTypeName("const char *")] sbyte* username, [NativeTypeName("size_t")] nuint username_len, [NativeTypeName("const char *")] sbyte* publickeyfiledata, [NativeTypeName("size_t")] nuint publickeyfiledata_len, [NativeTypeName("const char *")] sbyte* privatekeyfiledata, [NativeTypeName("size_t")] nuint privatekeyfiledata_len, [NativeTypeName("const char *")] sbyte* passphrase);

    [DllImport("libssh2", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
    public static extern int libssh2_userauth_keyboard_interactive_ex([NativeTypeName("LIBSSH2_SESSION *")] _LIBSSH2_SESSION* session, [NativeTypeName("const char *")] sbyte* username, [NativeTypeName("unsigned int")] uint username_len, [NativeTypeName("void (*)(const char *, int, const char *, int, int, const LIBSSH2_USERAUTH_KBDINT_PROMPT *, LIBSSH2_USERAUTH_KBDINT_RESPONSE *, void **)")] delegate* unmanaged[Cdecl]<sbyte*, int, sbyte*, int, int, _LIBSSH2_USERAUTH_KBDINT_PROMPT*, _LIBSSH2_USERAUTH_KBDINT_RESPONSE*, void**, void> response_callback);

    [DllImport("libssh2", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
    public static extern int libssh2_userauth_publickey_sk([NativeTypeName("LIBSSH2_SESSION *")] _LIBSSH2_SESSION* session, [NativeTypeName("const char *")] sbyte* username, [NativeTypeName("size_t")] nuint username_len, [NativeTypeName("const unsigned char *")] byte* pubkeydata, [NativeTypeName("size_t")] nuint pubkeydata_len, [NativeTypeName("const char *")] sbyte* privatekeydata, [NativeTypeName("size_t")] nuint privatekeydata_len, [NativeTypeName("const char *")] sbyte* passphrase, [NativeTypeName("int (*)(LIBSSH2_SESSION *, LIBSSH2_SK_SIG_INFO *, const unsigned char *, size_t, int, uint8_t, const char *, const unsigned char *, size_t, void **)")] delegate* unmanaged[Cdecl]<_LIBSSH2_SESSION*, _LIBSSH2_SK_SIG_INFO*, byte*, nuint, int, byte, sbyte*, byte*, nuint, void**, int> sign_callback, void** @abstract);

    [DllImport("libssh2", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
    public static extern int libssh2_poll([NativeTypeName("LIBSSH2_POLLFD *")] _LIBSSH2_POLLFD* fds, [NativeTypeName("unsigned int")] uint nfds, [NativeTypeName("long")] int timeout);

    [DllImport("libssh2", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
    [return: NativeTypeName("LIBSSH2_CHANNEL *")]
    public static extern _LIBSSH2_CHANNEL* libssh2_channel_open_ex([NativeTypeName("LIBSSH2_SESSION *")] _LIBSSH2_SESSION* session, [NativeTypeName("const char *")] sbyte* channel_type, [NativeTypeName("unsigned int")] uint channel_type_len, [NativeTypeName("unsigned int")] uint window_size, [NativeTypeName("unsigned int")] uint packet_size, [NativeTypeName("const char *")] sbyte* message, [NativeTypeName("unsigned int")] uint message_len);

    [DllImport("libssh2", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
    [return: NativeTypeName("LIBSSH2_CHANNEL *")]
    public static extern _LIBSSH2_CHANNEL* libssh2_channel_direct_tcpip_ex([NativeTypeName("LIBSSH2_SESSION *")] _LIBSSH2_SESSION* session, [NativeTypeName("const char *")] sbyte* host, int port, [NativeTypeName("const char *")] sbyte* shost, int sport);

    [DllImport("libssh2", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
    [return: NativeTypeName("LIBSSH2_CHANNEL *")]
    public static extern _LIBSSH2_CHANNEL* libssh2_channel_direct_streamlocal_ex([NativeTypeName("LIBSSH2_SESSION *")] _LIBSSH2_SESSION* session, [NativeTypeName("const char *")] sbyte* socket_path, [NativeTypeName("const char *")] sbyte* shost, int sport);

    [DllImport("libssh2", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
    [return: NativeTypeName("LIBSSH2_LISTENER *")]
    public static extern _LIBSSH2_LISTENER* libssh2_channel_forward_listen_ex([NativeTypeName("LIBSSH2_SESSION *")] _LIBSSH2_SESSION* session, [NativeTypeName("const char *")] sbyte* host, int port, int* bound_port, int queue_maxsize);

    [DllImport("libssh2", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
    public static extern int libssh2_channel_forward_cancel([NativeTypeName("LIBSSH2_LISTENER *")] _LIBSSH2_LISTENER* listener);

    [DllImport("libssh2", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
    [return: NativeTypeName("LIBSSH2_CHANNEL *")]
    public static extern _LIBSSH2_CHANNEL* libssh2_channel_forward_accept([NativeTypeName("LIBSSH2_LISTENER *")] _LIBSSH2_LISTENER* listener);

    [DllImport("libssh2", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
    public static extern int libssh2_channel_setenv_ex([NativeTypeName("LIBSSH2_CHANNEL *")] _LIBSSH2_CHANNEL* channel, [NativeTypeName("const char *")] sbyte* varname, [NativeTypeName("unsigned int")] uint varname_len, [NativeTypeName("const char *")] sbyte* value, [NativeTypeName("unsigned int")] uint value_len);

    [DllImport("libssh2", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
    public static extern int libssh2_channel_request_auth_agent([NativeTypeName("LIBSSH2_CHANNEL *")] _LIBSSH2_CHANNEL* channel);

    [DllImport("libssh2", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
    public static extern int libssh2_channel_request_pty_ex([NativeTypeName("LIBSSH2_CHANNEL *")] _LIBSSH2_CHANNEL* channel, [NativeTypeName("const char *")] sbyte* term, [NativeTypeName("unsigned int")] uint term_len, [NativeTypeName("const char *")] sbyte* modes, [NativeTypeName("unsigned int")] uint modes_len, int width, int height, int width_px, int height_px);

    [DllImport("libssh2", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
    public static extern int libssh2_channel_request_pty_size_ex([NativeTypeName("LIBSSH2_CHANNEL *")] _LIBSSH2_CHANNEL* channel, int width, int height, int width_px, int height_px);

    [DllImport("libssh2", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
    public static extern int libssh2_channel_x11_req_ex([NativeTypeName("LIBSSH2_CHANNEL *")] _LIBSSH2_CHANNEL* channel, int single_connection, [NativeTypeName("const char *")] sbyte* auth_proto, [NativeTypeName("const char *")] sbyte* auth_cookie, int screen_number);

    [DllImport("libssh2", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
    public static extern int libssh2_channel_signal_ex([NativeTypeName("LIBSSH2_CHANNEL *")] _LIBSSH2_CHANNEL* channel, [NativeTypeName("const char *")] sbyte* signame, [NativeTypeName("size_t")] nuint signame_len);

    [DllImport("libssh2", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
    public static extern int libssh2_channel_process_startup([NativeTypeName("LIBSSH2_CHANNEL *")] _LIBSSH2_CHANNEL* channel, [NativeTypeName("const char *")] sbyte* request, [NativeTypeName("unsigned int")] uint request_len, [NativeTypeName("const char *")] sbyte* message, [NativeTypeName("unsigned int")] uint message_len);

    [DllImport("libssh2", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
    [return: NativeTypeName("ssize_t")]
    public static extern long libssh2_channel_read_ex([NativeTypeName("LIBSSH2_CHANNEL *")] _LIBSSH2_CHANNEL* channel, int stream_id, [NativeTypeName("char *")] sbyte* buf, [NativeTypeName("size_t")] nuint buflen);

    [DllImport("libssh2", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
    public static extern int libssh2_poll_channel_read([NativeTypeName("LIBSSH2_CHANNEL *")] _LIBSSH2_CHANNEL* channel, int extended);

    [DllImport("libssh2", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
    [return: NativeTypeName("unsigned long")]
    public static extern uint libssh2_channel_window_read_ex([NativeTypeName("LIBSSH2_CHANNEL *")] _LIBSSH2_CHANNEL* channel, [NativeTypeName("unsigned long *")] uint* read_avail, [NativeTypeName("unsigned long *")] uint* window_size_initial);

    [DllImport("libssh2", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
    [return: NativeTypeName("unsigned long")]
    [Obsolete("Use libssh2_channel_receive_window_adjust2()")]
    public static extern uint libssh2_channel_receive_window_adjust([NativeTypeName("LIBSSH2_CHANNEL *")] _LIBSSH2_CHANNEL* channel, [NativeTypeName("unsigned long")] uint adjustment, [NativeTypeName("unsigned char")] byte force);

    [DllImport("libssh2", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
    public static extern int libssh2_channel_receive_window_adjust2([NativeTypeName("LIBSSH2_CHANNEL *")] _LIBSSH2_CHANNEL* channel, [NativeTypeName("unsigned long")] uint adjustment, [NativeTypeName("unsigned char")] byte force, [NativeTypeName("unsigned int *")] uint* storewindow);

    [DllImport("libssh2", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
    [return: NativeTypeName("ssize_t")]
    public static extern long libssh2_channel_write_ex([NativeTypeName("LIBSSH2_CHANNEL *")] _LIBSSH2_CHANNEL* channel, int stream_id, [NativeTypeName("const char *")] sbyte* buf, [NativeTypeName("size_t")] nuint buflen);

    [DllImport("libssh2", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
    [return: NativeTypeName("unsigned long")]
    public static extern uint libssh2_channel_window_write_ex([NativeTypeName("LIBSSH2_CHANNEL *")] _LIBSSH2_CHANNEL* channel, [NativeTypeName("unsigned long *")] uint* window_size_initial);

    [DllImport("libssh2", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
    public static extern void libssh2_session_set_blocking([NativeTypeName("LIBSSH2_SESSION *")] _LIBSSH2_SESSION* session, int blocking);

    [DllImport("libssh2", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
    public static extern int libssh2_session_get_blocking([NativeTypeName("LIBSSH2_SESSION *")] _LIBSSH2_SESSION* session);

    [DllImport("libssh2", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
    public static extern void libssh2_channel_set_blocking([NativeTypeName("LIBSSH2_CHANNEL *")] _LIBSSH2_CHANNEL* channel, int blocking);

    [DllImport("libssh2", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
    public static extern void libssh2_session_set_timeout([NativeTypeName("LIBSSH2_SESSION *")] _LIBSSH2_SESSION* session, [NativeTypeName("long")] int timeout);

    [DllImport("libssh2", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
    [return: NativeTypeName("long")]
    public static extern int libssh2_session_get_timeout([NativeTypeName("LIBSSH2_SESSION *")] _LIBSSH2_SESSION* session);

    [DllImport("libssh2", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
    public static extern void libssh2_session_set_read_timeout([NativeTypeName("LIBSSH2_SESSION *")] _LIBSSH2_SESSION* session, [NativeTypeName("long")] int timeout);

    [DllImport("libssh2", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
    [return: NativeTypeName("long")]
    public static extern int libssh2_session_get_read_timeout([NativeTypeName("LIBSSH2_SESSION *")] _LIBSSH2_SESSION* session);

    [DllImport("libssh2", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
    [Obsolete("libssh2_channel_handle_extended_data2()")]
    public static extern void libssh2_channel_handle_extended_data([NativeTypeName("LIBSSH2_CHANNEL *")] _LIBSSH2_CHANNEL* channel, int ignore_mode);

    [DllImport("libssh2", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
    public static extern int libssh2_channel_handle_extended_data2([NativeTypeName("LIBSSH2_CHANNEL *")] _LIBSSH2_CHANNEL* channel, int ignore_mode);

    [DllImport("libssh2", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
    public static extern int libssh2_channel_flush_ex([NativeTypeName("LIBSSH2_CHANNEL *")] _LIBSSH2_CHANNEL* channel, int streamid);

    [DllImport("libssh2", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
    public static extern int libssh2_channel_get_exit_status([NativeTypeName("LIBSSH2_CHANNEL *")] _LIBSSH2_CHANNEL* channel);

    [DllImport("libssh2", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
    public static extern int libssh2_channel_get_exit_signal([NativeTypeName("LIBSSH2_CHANNEL *")] _LIBSSH2_CHANNEL* channel, [NativeTypeName("char **")] sbyte** exitsignal, [NativeTypeName("size_t *")] nuint* exitsignal_len, [NativeTypeName("char **")] sbyte** errmsg, [NativeTypeName("size_t *")] nuint* errmsg_len, [NativeTypeName("char **")] sbyte** langtag, [NativeTypeName("size_t *")] nuint* langtag_len);

    [DllImport("libssh2", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
    public static extern int libssh2_channel_send_eof([NativeTypeName("LIBSSH2_CHANNEL *")] _LIBSSH2_CHANNEL* channel);

    [DllImport("libssh2", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
    public static extern int libssh2_channel_eof([NativeTypeName("LIBSSH2_CHANNEL *")] _LIBSSH2_CHANNEL* channel);

    [DllImport("libssh2", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
    public static extern int libssh2_channel_wait_eof([NativeTypeName("LIBSSH2_CHANNEL *")] _LIBSSH2_CHANNEL* channel);

    [DllImport("libssh2", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
    public static extern int libssh2_channel_close([NativeTypeName("LIBSSH2_CHANNEL *")] _LIBSSH2_CHANNEL* channel);

    [DllImport("libssh2", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
    public static extern int libssh2_channel_wait_closed([NativeTypeName("LIBSSH2_CHANNEL *")] _LIBSSH2_CHANNEL* channel);

    [DllImport("libssh2", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
    public static extern int libssh2_channel_free([NativeTypeName("LIBSSH2_CHANNEL *")] _LIBSSH2_CHANNEL* channel);

    [DllImport("libssh2", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
    [return: NativeTypeName("LIBSSH2_CHANNEL *")]
    [Obsolete("Use libssh2_scp_recv2()")]
    public static extern _LIBSSH2_CHANNEL* libssh2_scp_recv([NativeTypeName("LIBSSH2_SESSION *")] _LIBSSH2_SESSION* session, [NativeTypeName("const char *")] sbyte* path, [NativeTypeName("struct stat *")] void* sb);

    [DllImport("libssh2", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
    [return: NativeTypeName("LIBSSH2_CHANNEL *")]
    public static extern _LIBSSH2_CHANNEL* libssh2_scp_recv2([NativeTypeName("LIBSSH2_SESSION *")] _LIBSSH2_SESSION* session, [NativeTypeName("const char *")] sbyte* path, [NativeTypeName("libssh2_struct_stat *")] void* sb);

    [DllImport("libssh2", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
    [return: NativeTypeName("LIBSSH2_CHANNEL *")]
    public static extern _LIBSSH2_CHANNEL* libssh2_scp_send_ex([NativeTypeName("LIBSSH2_SESSION *")] _LIBSSH2_SESSION* session, [NativeTypeName("const char *")] sbyte* path, int mode, [NativeTypeName("size_t")] nuint size, [NativeTypeName("long")] int mtime, [NativeTypeName("long")] int atime);

    [DllImport("libssh2", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
    [return: NativeTypeName("LIBSSH2_CHANNEL *")]
    public static extern _LIBSSH2_CHANNEL* libssh2_scp_send64([NativeTypeName("LIBSSH2_SESSION *")] _LIBSSH2_SESSION* session, [NativeTypeName("const char *")] sbyte* path, int mode, [NativeTypeName("libssh2_int64_t")] long size, [NativeTypeName("time_t")] long mtime, [NativeTypeName("time_t")] long atime);

    [DllImport("libssh2", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
    public static extern int libssh2_base64_decode([NativeTypeName("LIBSSH2_SESSION *")] _LIBSSH2_SESSION* session, [NativeTypeName("char **")] sbyte** dest, [NativeTypeName("unsigned int *")] uint* dest_len, [NativeTypeName("const char *")] sbyte* src, [NativeTypeName("unsigned int")] uint src_len);

    [DllImport("libssh2", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
    [return: NativeTypeName("const char *")]
    public static extern sbyte* libssh2_version(int req_version_num);

    [DllImport("libssh2", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
    public static extern libssh2_crypto_engine_t libssh2_crypto_engine();

    [DllImport("libssh2", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
    [return: NativeTypeName("LIBSSH2_KNOWNHOSTS *")]
    public static extern _LIBSSH2_KNOWNHOSTS* libssh2_knownhost_init([NativeTypeName("LIBSSH2_SESSION *")] _LIBSSH2_SESSION* session);

    [DllImport("libssh2", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
    public static extern int libssh2_knownhost_add([NativeTypeName("LIBSSH2_KNOWNHOSTS *")] _LIBSSH2_KNOWNHOSTS* hosts, [NativeTypeName("const char *")] sbyte* host, [NativeTypeName("const char *")] sbyte* salt, [NativeTypeName("const char *")] sbyte* key, [NativeTypeName("size_t")] nuint keylen, int typemask, [NativeTypeName("struct libssh2_knownhost **")] libssh2_knownhost** store);

    [DllImport("libssh2", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
    public static extern int libssh2_knownhost_addc([NativeTypeName("LIBSSH2_KNOWNHOSTS *")] _LIBSSH2_KNOWNHOSTS* hosts, [NativeTypeName("const char *")] sbyte* host, [NativeTypeName("const char *")] sbyte* salt, [NativeTypeName("const char *")] sbyte* key, [NativeTypeName("size_t")] nuint keylen, [NativeTypeName("const char *")] sbyte* comment, [NativeTypeName("size_t")] nuint commentlen, int typemask, [NativeTypeName("struct libssh2_knownhost **")] libssh2_knownhost** store);

    [DllImport("libssh2", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
    public static extern int libssh2_knownhost_check([NativeTypeName("LIBSSH2_KNOWNHOSTS *")] _LIBSSH2_KNOWNHOSTS* hosts, [NativeTypeName("const char *")] sbyte* host, [NativeTypeName("const char *")] sbyte* key, [NativeTypeName("size_t")] nuint keylen, int typemask, [NativeTypeName("struct libssh2_knownhost **")] libssh2_knownhost** knownhost);

    [DllImport("libssh2", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
    public static extern int libssh2_knownhost_checkp([NativeTypeName("LIBSSH2_KNOWNHOSTS *")] _LIBSSH2_KNOWNHOSTS* hosts, [NativeTypeName("const char *")] sbyte* host, int port, [NativeTypeName("const char *")] sbyte* key, [NativeTypeName("size_t")] nuint keylen, int typemask, [NativeTypeName("struct libssh2_knownhost **")] libssh2_knownhost** knownhost);

    [DllImport("libssh2", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
    public static extern int libssh2_knownhost_del([NativeTypeName("LIBSSH2_KNOWNHOSTS *")] _LIBSSH2_KNOWNHOSTS* hosts, [NativeTypeName("struct libssh2_knownhost *")] libssh2_knownhost* entry);

    [DllImport("libssh2", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
    public static extern void libssh2_knownhost_free([NativeTypeName("LIBSSH2_KNOWNHOSTS *")] _LIBSSH2_KNOWNHOSTS* hosts);

    [DllImport("libssh2", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
    public static extern int libssh2_knownhost_readline([NativeTypeName("LIBSSH2_KNOWNHOSTS *")] _LIBSSH2_KNOWNHOSTS* hosts, [NativeTypeName("const char *")] sbyte* line, [NativeTypeName("size_t")] nuint len, int type);

    [DllImport("libssh2", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
    public static extern int libssh2_knownhost_readfile([NativeTypeName("LIBSSH2_KNOWNHOSTS *")] _LIBSSH2_KNOWNHOSTS* hosts, [NativeTypeName("const char *")] sbyte* filename, int type);

    [DllImport("libssh2", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
    public static extern int libssh2_knownhost_writeline([NativeTypeName("LIBSSH2_KNOWNHOSTS *")] _LIBSSH2_KNOWNHOSTS* hosts, [NativeTypeName("struct libssh2_knownhost *")] libssh2_knownhost* known, [NativeTypeName("char *")] sbyte* buffer, [NativeTypeName("size_t")] nuint buflen, [NativeTypeName("size_t *")] nuint* outlen, int type);

    [DllImport("libssh2", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
    public static extern int libssh2_knownhost_writefile([NativeTypeName("LIBSSH2_KNOWNHOSTS *")] _LIBSSH2_KNOWNHOSTS* hosts, [NativeTypeName("const char *")] sbyte* filename, int type);

    [DllImport("libssh2", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
    public static extern int libssh2_knownhost_get([NativeTypeName("LIBSSH2_KNOWNHOSTS *")] _LIBSSH2_KNOWNHOSTS* hosts, [NativeTypeName("struct libssh2_knownhost **")] libssh2_knownhost** store, [NativeTypeName("struct libssh2_knownhost *")] libssh2_knownhost* prev);

    [DllImport("libssh2", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
    [return: NativeTypeName("LIBSSH2_AGENT *")]
    public static extern _LIBSSH2_AGENT* libssh2_agent_init([NativeTypeName("LIBSSH2_SESSION *")] _LIBSSH2_SESSION* session);

    [DllImport("libssh2", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
    public static extern int libssh2_agent_connect([NativeTypeName("LIBSSH2_AGENT *")] _LIBSSH2_AGENT* agent);

    [DllImport("libssh2", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
    public static extern int libssh2_agent_list_identities([NativeTypeName("LIBSSH2_AGENT *")] _LIBSSH2_AGENT* agent);

    [DllImport("libssh2", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
    public static extern int libssh2_agent_get_identity([NativeTypeName("LIBSSH2_AGENT *")] _LIBSSH2_AGENT* agent, [NativeTypeName("struct libssh2_agent_publickey **")] libssh2_agent_publickey** store, [NativeTypeName("struct libssh2_agent_publickey *")] libssh2_agent_publickey* prev);

    [DllImport("libssh2", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
    public static extern int libssh2_agent_userauth([NativeTypeName("LIBSSH2_AGENT *")] _LIBSSH2_AGENT* agent, [NativeTypeName("const char *")] sbyte* username, [NativeTypeName("struct libssh2_agent_publickey *")] libssh2_agent_publickey* identity);

    [DllImport("libssh2", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
    public static extern int libssh2_agent_sign([NativeTypeName("LIBSSH2_AGENT *")] _LIBSSH2_AGENT* agent, [NativeTypeName("struct libssh2_agent_publickey *")] libssh2_agent_publickey* identity, [NativeTypeName("unsigned char **")] byte** sig, [NativeTypeName("size_t *")] nuint* s_len, [NativeTypeName("const unsigned char *")] byte* data, [NativeTypeName("size_t")] nuint d_len, [NativeTypeName("const char *")] sbyte* method, [NativeTypeName("unsigned int")] uint method_len);

    [DllImport("libssh2", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
    public static extern int libssh2_agent_disconnect([NativeTypeName("LIBSSH2_AGENT *")] _LIBSSH2_AGENT* agent);

    [DllImport("libssh2", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
    public static extern void libssh2_agent_free([NativeTypeName("LIBSSH2_AGENT *")] _LIBSSH2_AGENT* agent);

    [DllImport("libssh2", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
    public static extern void libssh2_agent_set_identity_path([NativeTypeName("LIBSSH2_AGENT *")] _LIBSSH2_AGENT* agent, [NativeTypeName("const char *")] sbyte* path);

    [DllImport("libssh2", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
    [return: NativeTypeName("const char *")]
    public static extern sbyte* libssh2_agent_get_identity_path([NativeTypeName("LIBSSH2_AGENT *")] _LIBSSH2_AGENT* agent);

    [DllImport("libssh2", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
    public static extern void libssh2_keepalive_config([NativeTypeName("LIBSSH2_SESSION *")] _LIBSSH2_SESSION* session, int want_reply, [NativeTypeName("unsigned int")] uint interval);

    [DllImport("libssh2", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
    public static extern int libssh2_keepalive_send([NativeTypeName("LIBSSH2_SESSION *")] _LIBSSH2_SESSION* session, int* seconds_to_next);

    [DllImport("libssh2", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
    public static extern int libssh2_trace([NativeTypeName("LIBSSH2_SESSION *")] _LIBSSH2_SESSION* session, int bitmask);

    [DllImport("libssh2", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
    public static extern int libssh2_trace_sethandler([NativeTypeName("LIBSSH2_SESSION *")] _LIBSSH2_SESSION* session, void* context, [NativeTypeName("libssh2_trace_handler_func")] delegate* unmanaged[Cdecl]<_LIBSSH2_SESSION*, void*, sbyte*, nuint, void> callback);

    [NativeTypeName("#define LIBSSH2_H 1")]
    public const int LIBSSH2_H = 1;

    [NativeTypeName("#define LIBSSH2_COPYRIGHT \"The libssh2 project and its contributors.\"")]
    public static ReadOnlySpan<byte> LIBSSH2_COPYRIGHT => "The libssh2 project and its contributors."u8;

    [NativeTypeName("#define LIBSSH2_VERSION \"1.11.1_DEV\"")]
    public static ReadOnlySpan<byte> LIBSSH2_VERSION => "1.11.1_DEV"u8;

    [NativeTypeName("#define LIBSSH2_VERSION_MAJOR 1")]
    public const int LIBSSH2_VERSION_MAJOR = 1;

    [NativeTypeName("#define LIBSSH2_VERSION_MINOR 11")]
    public const int LIBSSH2_VERSION_MINOR = 11;

    [NativeTypeName("#define LIBSSH2_VERSION_PATCH 1")]
    public const int LIBSSH2_VERSION_PATCH = 1;

    [NativeTypeName("#define LIBSSH2_VERSION_NUM 0x010b01")]
    public const int LIBSSH2_VERSION_NUM = 0x010b01;

    [NativeTypeName("#define LIBSSH2_TIMESTAMP \"DEV\"")]
    public static ReadOnlySpan<byte> LIBSSH2_TIMESTAMP => "DEV"u8;

    [NativeTypeName("#define LIBSSH2_INVALID_SOCKET INVALID_SOCKET")]
    public const ulong LIBSSH2_INVALID_SOCKET = unchecked((ulong)(~0));

    [NativeTypeName("#define LIBSSH2_STRUCT_STAT_SIZE_FORMAT \"%I64d\"")]
    public static ReadOnlySpan<byte> LIBSSH2_STRUCT_STAT_SIZE_FORMAT => "%I64d"u8;

    [NativeTypeName("#define LIBSSH2_SSH_BANNER \"SSH-2.0-libssh2_\" LIBSSH2_VERSION")]
    public static ReadOnlySpan<byte> LIBSSH2_SSH_BANNER => "SSH-2.0-libssh2_1.11.1_DEV"u8;

    [NativeTypeName("#define LIBSSH2_SSH_DEFAULT_BANNER LIBSSH2_SSH_BANNER")]
    public static ReadOnlySpan<byte> LIBSSH2_SSH_DEFAULT_BANNER => "SSH-2.0-libssh2_1.11.1_DEV"u8;

    [NativeTypeName("#define LIBSSH2_SSH_DEFAULT_BANNER_WITH_CRLF LIBSSH2_SSH_DEFAULT_BANNER \"\r\n\"")]
    public static ReadOnlySpan<byte> LIBSSH2_SSH_DEFAULT_BANNER_WITH_CRLF => "SSH-2.0-libssh2_1.11.1_DEV\r\n"u8;

    [NativeTypeName("#define LIBSSH2_TERM_WIDTH 80")]
    public const int LIBSSH2_TERM_WIDTH = 80;

    [NativeTypeName("#define LIBSSH2_TERM_HEIGHT 24")]
    public const int LIBSSH2_TERM_HEIGHT = 24;

    [NativeTypeName("#define LIBSSH2_TERM_WIDTH_PX 0")]
    public const int LIBSSH2_TERM_WIDTH_PX = 0;

    [NativeTypeName("#define LIBSSH2_TERM_HEIGHT_PX 0")]
    public const int LIBSSH2_TERM_HEIGHT_PX = 0;

    [NativeTypeName("#define LIBSSH2_SOCKET_POLL_UDELAY 250000")]
    public const int LIBSSH2_SOCKET_POLL_UDELAY = 250000;

    [NativeTypeName("#define LIBSSH2_SOCKET_POLL_MAXLOOPS 120")]
    public const int LIBSSH2_SOCKET_POLL_MAXLOOPS = 120;

    [NativeTypeName("#define LIBSSH2_PACKET_MAXCOMP 32000")]
    public const int LIBSSH2_PACKET_MAXCOMP = 32000;

    [NativeTypeName("#define LIBSSH2_PACKET_MAXDECOMP 40000")]
    public const int LIBSSH2_PACKET_MAXDECOMP = 40000;

    [NativeTypeName("#define LIBSSH2_PACKET_MAXPAYLOAD 40000")]
    public const int LIBSSH2_PACKET_MAXPAYLOAD = 40000;

    [NativeTypeName("#define LIBSSH2_SK_PRESENCE_REQUIRED 0x01")]
    public const int LIBSSH2_SK_PRESENCE_REQUIRED = 0x01;

    [NativeTypeName("#define LIBSSH2_SK_VERIFICATION_REQUIRED 0x04")]
    public const int LIBSSH2_SK_VERIFICATION_REQUIRED = 0x04;

    [NativeTypeName("#define LIBSSH2_CALLBACK_IGNORE 0")]
    public const int LIBSSH2_CALLBACK_IGNORE = 0;

    [NativeTypeName("#define LIBSSH2_CALLBACK_DEBUG 1")]
    public const int LIBSSH2_CALLBACK_DEBUG = 1;

    [NativeTypeName("#define LIBSSH2_CALLBACK_DISCONNECT 2")]
    public const int LIBSSH2_CALLBACK_DISCONNECT = 2;

    [NativeTypeName("#define LIBSSH2_CALLBACK_MACERROR 3")]
    public const int LIBSSH2_CALLBACK_MACERROR = 3;

    [NativeTypeName("#define LIBSSH2_CALLBACK_X11 4")]
    public const int LIBSSH2_CALLBACK_X11 = 4;

    [NativeTypeName("#define LIBSSH2_CALLBACK_SEND 5")]
    public const int LIBSSH2_CALLBACK_SEND = 5;

    [NativeTypeName("#define LIBSSH2_CALLBACK_RECV 6")]
    public const int LIBSSH2_CALLBACK_RECV = 6;

    [NativeTypeName("#define LIBSSH2_CALLBACK_AUTHAGENT 7")]
    public const int LIBSSH2_CALLBACK_AUTHAGENT = 7;

    [NativeTypeName("#define LIBSSH2_CALLBACK_AUTHAGENT_IDENTITIES 8")]
    public const int LIBSSH2_CALLBACK_AUTHAGENT_IDENTITIES = 8;

    [NativeTypeName("#define LIBSSH2_CALLBACK_AUTHAGENT_SIGN 9")]
    public const int LIBSSH2_CALLBACK_AUTHAGENT_SIGN = 9;

    [NativeTypeName("#define LIBSSH2_METHOD_KEX 0")]
    public const int LIBSSH2_METHOD_KEX = 0;

    [NativeTypeName("#define LIBSSH2_METHOD_HOSTKEY 1")]
    public const int LIBSSH2_METHOD_HOSTKEY = 1;

    [NativeTypeName("#define LIBSSH2_METHOD_CRYPT_CS 2")]
    public const int LIBSSH2_METHOD_CRYPT_CS = 2;

    [NativeTypeName("#define LIBSSH2_METHOD_CRYPT_SC 3")]
    public const int LIBSSH2_METHOD_CRYPT_SC = 3;

    [NativeTypeName("#define LIBSSH2_METHOD_MAC_CS 4")]
    public const int LIBSSH2_METHOD_MAC_CS = 4;

    [NativeTypeName("#define LIBSSH2_METHOD_MAC_SC 5")]
    public const int LIBSSH2_METHOD_MAC_SC = 5;

    [NativeTypeName("#define LIBSSH2_METHOD_COMP_CS 6")]
    public const int LIBSSH2_METHOD_COMP_CS = 6;

    [NativeTypeName("#define LIBSSH2_METHOD_COMP_SC 7")]
    public const int LIBSSH2_METHOD_COMP_SC = 7;

    [NativeTypeName("#define LIBSSH2_METHOD_LANG_CS 8")]
    public const int LIBSSH2_METHOD_LANG_CS = 8;

    [NativeTypeName("#define LIBSSH2_METHOD_LANG_SC 9")]
    public const int LIBSSH2_METHOD_LANG_SC = 9;

    [NativeTypeName("#define LIBSSH2_METHOD_SIGN_ALGO 10")]
    public const int LIBSSH2_METHOD_SIGN_ALGO = 10;

    [NativeTypeName("#define LIBSSH2_FLAG_SIGPIPE 1")]
    public const int LIBSSH2_FLAG_SIGPIPE = 1;

    [NativeTypeName("#define LIBSSH2_FLAG_COMPRESS 2")]
    public const int LIBSSH2_FLAG_COMPRESS = 2;

    [NativeTypeName("#define LIBSSH2_FLAG_QUOTE_PATHS 3")]
    public const int LIBSSH2_FLAG_QUOTE_PATHS = 3;

    [NativeTypeName("#define LIBSSH2_POLLFD_SOCKET 1")]
    public const int LIBSSH2_POLLFD_SOCKET = 1;

    [NativeTypeName("#define LIBSSH2_POLLFD_CHANNEL 2")]
    public const int LIBSSH2_POLLFD_CHANNEL = 2;

    [NativeTypeName("#define LIBSSH2_POLLFD_LISTENER 3")]
    public const int LIBSSH2_POLLFD_LISTENER = 3;

    [NativeTypeName("#define LIBSSH2_POLLFD_POLLIN 0x0001")]
    public const int LIBSSH2_POLLFD_POLLIN = 0x0001;

    [NativeTypeName("#define LIBSSH2_POLLFD_POLLPRI 0x0002")]
    public const int LIBSSH2_POLLFD_POLLPRI = 0x0002;

    [NativeTypeName("#define LIBSSH2_POLLFD_POLLEXT 0x0002")]
    public const int LIBSSH2_POLLFD_POLLEXT = 0x0002;

    [NativeTypeName("#define LIBSSH2_POLLFD_POLLOUT 0x0004")]
    public const int LIBSSH2_POLLFD_POLLOUT = 0x0004;

    [NativeTypeName("#define LIBSSH2_POLLFD_POLLERR 0x0008")]
    public const int LIBSSH2_POLLFD_POLLERR = 0x0008;

    [NativeTypeName("#define LIBSSH2_POLLFD_POLLHUP 0x0010")]
    public const int LIBSSH2_POLLFD_POLLHUP = 0x0010;

    [NativeTypeName("#define LIBSSH2_POLLFD_SESSION_CLOSED 0x0010")]
    public const int LIBSSH2_POLLFD_SESSION_CLOSED = 0x0010;

    [NativeTypeName("#define LIBSSH2_POLLFD_POLLNVAL 0x0020")]
    public const int LIBSSH2_POLLFD_POLLNVAL = 0x0020;

    [NativeTypeName("#define LIBSSH2_POLLFD_POLLEX 0x0040")]
    public const int LIBSSH2_POLLFD_POLLEX = 0x0040;

    [NativeTypeName("#define LIBSSH2_POLLFD_CHANNEL_CLOSED 0x0080")]
    public const int LIBSSH2_POLLFD_CHANNEL_CLOSED = 0x0080;

    [NativeTypeName("#define LIBSSH2_POLLFD_LISTENER_CLOSED 0x0080")]
    public const int LIBSSH2_POLLFD_LISTENER_CLOSED = 0x0080;

    [NativeTypeName("#define LIBSSH2_SESSION_BLOCK_INBOUND 0x0001")]
    public const int LIBSSH2_SESSION_BLOCK_INBOUND = 0x0001;

    [NativeTypeName("#define LIBSSH2_SESSION_BLOCK_OUTBOUND 0x0002")]
    public const int LIBSSH2_SESSION_BLOCK_OUTBOUND = 0x0002;

    [NativeTypeName("#define LIBSSH2_HOSTKEY_HASH_MD5 1")]
    public const int LIBSSH2_HOSTKEY_HASH_MD5 = 1;

    [NativeTypeName("#define LIBSSH2_HOSTKEY_HASH_SHA1 2")]
    public const int LIBSSH2_HOSTKEY_HASH_SHA1 = 2;

    [NativeTypeName("#define LIBSSH2_HOSTKEY_HASH_SHA256 3")]
    public const int LIBSSH2_HOSTKEY_HASH_SHA256 = 3;

    [NativeTypeName("#define LIBSSH2_HOSTKEY_TYPE_UNKNOWN 0")]
    public const int LIBSSH2_HOSTKEY_TYPE_UNKNOWN = 0;

    [NativeTypeName("#define LIBSSH2_HOSTKEY_TYPE_RSA 1")]
    public const int LIBSSH2_HOSTKEY_TYPE_RSA = 1;

    [NativeTypeName("#define LIBSSH2_HOSTKEY_TYPE_DSS 2")]
    public const int LIBSSH2_HOSTKEY_TYPE_DSS = 2;

    [NativeTypeName("#define LIBSSH2_HOSTKEY_TYPE_ECDSA_256 3")]
    public const int LIBSSH2_HOSTKEY_TYPE_ECDSA_256 = 3;

    [NativeTypeName("#define LIBSSH2_HOSTKEY_TYPE_ECDSA_384 4")]
    public const int LIBSSH2_HOSTKEY_TYPE_ECDSA_384 = 4;

    [NativeTypeName("#define LIBSSH2_HOSTKEY_TYPE_ECDSA_521 5")]
    public const int LIBSSH2_HOSTKEY_TYPE_ECDSA_521 = 5;

    [NativeTypeName("#define LIBSSH2_HOSTKEY_TYPE_ED25519 6")]
    public const int LIBSSH2_HOSTKEY_TYPE_ED25519 = 6;

    [NativeTypeName("#define SSH_DISCONNECT_HOST_NOT_ALLOWED_TO_CONNECT 1")]
    public const int SSH_DISCONNECT_HOST_NOT_ALLOWED_TO_CONNECT = 1;

    [NativeTypeName("#define SSH_DISCONNECT_PROTOCOL_ERROR 2")]
    public const int SSH_DISCONNECT_PROTOCOL_ERROR = 2;

    [NativeTypeName("#define SSH_DISCONNECT_KEY_EXCHANGE_FAILED 3")]
    public const int SSH_DISCONNECT_KEY_EXCHANGE_FAILED = 3;

    [NativeTypeName("#define SSH_DISCONNECT_RESERVED 4")]
    public const int SSH_DISCONNECT_RESERVED = 4;

    [NativeTypeName("#define SSH_DISCONNECT_MAC_ERROR 5")]
    public const int SSH_DISCONNECT_MAC_ERROR = 5;

    [NativeTypeName("#define SSH_DISCONNECT_COMPRESSION_ERROR 6")]
    public const int SSH_DISCONNECT_COMPRESSION_ERROR = 6;

    [NativeTypeName("#define SSH_DISCONNECT_SERVICE_NOT_AVAILABLE 7")]
    public const int SSH_DISCONNECT_SERVICE_NOT_AVAILABLE = 7;

    [NativeTypeName("#define SSH_DISCONNECT_PROTOCOL_VERSION_NOT_SUPPORTED 8")]
    public const int SSH_DISCONNECT_PROTOCOL_VERSION_NOT_SUPPORTED = 8;

    [NativeTypeName("#define SSH_DISCONNECT_HOST_KEY_NOT_VERIFIABLE 9")]
    public const int SSH_DISCONNECT_HOST_KEY_NOT_VERIFIABLE = 9;

    [NativeTypeName("#define SSH_DISCONNECT_CONNECTION_LOST 10")]
    public const int SSH_DISCONNECT_CONNECTION_LOST = 10;

    [NativeTypeName("#define SSH_DISCONNECT_BY_APPLICATION 11")]
    public const int SSH_DISCONNECT_BY_APPLICATION = 11;

    [NativeTypeName("#define SSH_DISCONNECT_TOO_MANY_CONNECTIONS 12")]
    public const int SSH_DISCONNECT_TOO_MANY_CONNECTIONS = 12;

    [NativeTypeName("#define SSH_DISCONNECT_AUTH_CANCELLED_BY_USER 13")]
    public const int SSH_DISCONNECT_AUTH_CANCELLED_BY_USER = 13;

    [NativeTypeName("#define SSH_DISCONNECT_NO_MORE_AUTH_METHODS_AVAILABLE 14")]
    public const int SSH_DISCONNECT_NO_MORE_AUTH_METHODS_AVAILABLE = 14;

    [NativeTypeName("#define SSH_DISCONNECT_ILLEGAL_USER_NAME 15")]
    public const int SSH_DISCONNECT_ILLEGAL_USER_NAME = 15;

    [NativeTypeName("#define LIBSSH2_ERROR_NONE 0")]
    public const int LIBSSH2_ERROR_NONE = 0;

    [NativeTypeName("#define LIBSSH2_ERROR_SOCKET_NONE -1")]
    public const int LIBSSH2_ERROR_SOCKET_NONE = -1;

    [NativeTypeName("#define LIBSSH2_ERROR_BANNER_RECV -2")]
    public const int LIBSSH2_ERROR_BANNER_RECV = -2;

    [NativeTypeName("#define LIBSSH2_ERROR_BANNER_SEND -3")]
    public const int LIBSSH2_ERROR_BANNER_SEND = -3;

    [NativeTypeName("#define LIBSSH2_ERROR_INVALID_MAC -4")]
    public const int LIBSSH2_ERROR_INVALID_MAC = -4;

    [NativeTypeName("#define LIBSSH2_ERROR_KEX_FAILURE -5")]
    public const int LIBSSH2_ERROR_KEX_FAILURE = -5;

    [NativeTypeName("#define LIBSSH2_ERROR_ALLOC -6")]
    public const int LIBSSH2_ERROR_ALLOC = -6;

    [NativeTypeName("#define LIBSSH2_ERROR_SOCKET_SEND -7")]
    public const int LIBSSH2_ERROR_SOCKET_SEND = -7;

    [NativeTypeName("#define LIBSSH2_ERROR_KEY_EXCHANGE_FAILURE -8")]
    public const int LIBSSH2_ERROR_KEY_EXCHANGE_FAILURE = -8;

    [NativeTypeName("#define LIBSSH2_ERROR_TIMEOUT -9")]
    public const int LIBSSH2_ERROR_TIMEOUT = -9;

    [NativeTypeName("#define LIBSSH2_ERROR_HOSTKEY_INIT -10")]
    public const int LIBSSH2_ERROR_HOSTKEY_INIT = -10;

    [NativeTypeName("#define LIBSSH2_ERROR_HOSTKEY_SIGN -11")]
    public const int LIBSSH2_ERROR_HOSTKEY_SIGN = -11;

    [NativeTypeName("#define LIBSSH2_ERROR_DECRYPT -12")]
    public const int LIBSSH2_ERROR_DECRYPT = -12;

    [NativeTypeName("#define LIBSSH2_ERROR_SOCKET_DISCONNECT -13")]
    public const int LIBSSH2_ERROR_SOCKET_DISCONNECT = -13;

    [NativeTypeName("#define LIBSSH2_ERROR_PROTO -14")]
    public const int LIBSSH2_ERROR_PROTO = -14;

    [NativeTypeName("#define LIBSSH2_ERROR_PASSWORD_EXPIRED -15")]
    public const int LIBSSH2_ERROR_PASSWORD_EXPIRED = -15;

    [NativeTypeName("#define LIBSSH2_ERROR_FILE -16")]
    public const int LIBSSH2_ERROR_FILE = -16;

    [NativeTypeName("#define LIBSSH2_ERROR_METHOD_NONE -17")]
    public const int LIBSSH2_ERROR_METHOD_NONE = -17;

    [NativeTypeName("#define LIBSSH2_ERROR_AUTHENTICATION_FAILED -18")]
    public const int LIBSSH2_ERROR_AUTHENTICATION_FAILED = -18;

    [NativeTypeName("#define LIBSSH2_ERROR_PUBLICKEY_UNRECOGNIZED LIBSSH2_ERROR_AUTHENTICATION_FAILED")]
    public const int LIBSSH2_ERROR_PUBLICKEY_UNRECOGNIZED = -18;

    [NativeTypeName("#define LIBSSH2_ERROR_PUBLICKEY_UNVERIFIED -19")]
    public const int LIBSSH2_ERROR_PUBLICKEY_UNVERIFIED = -19;

    [NativeTypeName("#define LIBSSH2_ERROR_CHANNEL_OUTOFORDER -20")]
    public const int LIBSSH2_ERROR_CHANNEL_OUTOFORDER = -20;

    [NativeTypeName("#define LIBSSH2_ERROR_CHANNEL_FAILURE -21")]
    public const int LIBSSH2_ERROR_CHANNEL_FAILURE = -21;

    [NativeTypeName("#define LIBSSH2_ERROR_CHANNEL_REQUEST_DENIED -22")]
    public const int LIBSSH2_ERROR_CHANNEL_REQUEST_DENIED = -22;

    [NativeTypeName("#define LIBSSH2_ERROR_CHANNEL_UNKNOWN -23")]
    public const int LIBSSH2_ERROR_CHANNEL_UNKNOWN = -23;

    [NativeTypeName("#define LIBSSH2_ERROR_CHANNEL_WINDOW_EXCEEDED -24")]
    public const int LIBSSH2_ERROR_CHANNEL_WINDOW_EXCEEDED = -24;

    [NativeTypeName("#define LIBSSH2_ERROR_CHANNEL_PACKET_EXCEEDED -25")]
    public const int LIBSSH2_ERROR_CHANNEL_PACKET_EXCEEDED = -25;

    [NativeTypeName("#define LIBSSH2_ERROR_CHANNEL_CLOSED -26")]
    public const int LIBSSH2_ERROR_CHANNEL_CLOSED = -26;

    [NativeTypeName("#define LIBSSH2_ERROR_CHANNEL_EOF_SENT -27")]
    public const int LIBSSH2_ERROR_CHANNEL_EOF_SENT = -27;

    [NativeTypeName("#define LIBSSH2_ERROR_SCP_PROTOCOL -28")]
    public const int LIBSSH2_ERROR_SCP_PROTOCOL = -28;

    [NativeTypeName("#define LIBSSH2_ERROR_ZLIB -29")]
    public const int LIBSSH2_ERROR_ZLIB = -29;

    [NativeTypeName("#define LIBSSH2_ERROR_SOCKET_TIMEOUT -30")]
    public const int LIBSSH2_ERROR_SOCKET_TIMEOUT = -30;

    [NativeTypeName("#define LIBSSH2_ERROR_SFTP_PROTOCOL -31")]
    public const int LIBSSH2_ERROR_SFTP_PROTOCOL = -31;

    [NativeTypeName("#define LIBSSH2_ERROR_REQUEST_DENIED -32")]
    public const int LIBSSH2_ERROR_REQUEST_DENIED = -32;

    [NativeTypeName("#define LIBSSH2_ERROR_METHOD_NOT_SUPPORTED -33")]
    public const int LIBSSH2_ERROR_METHOD_NOT_SUPPORTED = -33;

    [NativeTypeName("#define LIBSSH2_ERROR_INVAL -34")]
    public const int LIBSSH2_ERROR_INVAL = -34;

    [NativeTypeName("#define LIBSSH2_ERROR_INVALID_POLL_TYPE -35")]
    public const int LIBSSH2_ERROR_INVALID_POLL_TYPE = -35;

    [NativeTypeName("#define LIBSSH2_ERROR_PUBLICKEY_PROTOCOL -36")]
    public const int LIBSSH2_ERROR_PUBLICKEY_PROTOCOL = -36;

    [NativeTypeName("#define LIBSSH2_ERROR_EAGAIN -37")]
    public const int LIBSSH2_ERROR_EAGAIN = -37;

    [NativeTypeName("#define LIBSSH2_ERROR_BUFFER_TOO_SMALL -38")]
    public const int LIBSSH2_ERROR_BUFFER_TOO_SMALL = -38;

    [NativeTypeName("#define LIBSSH2_ERROR_BAD_USE -39")]
    public const int LIBSSH2_ERROR_BAD_USE = -39;

    [NativeTypeName("#define LIBSSH2_ERROR_COMPRESS -40")]
    public const int LIBSSH2_ERROR_COMPRESS = -40;

    [NativeTypeName("#define LIBSSH2_ERROR_OUT_OF_BOUNDARY -41")]
    public const int LIBSSH2_ERROR_OUT_OF_BOUNDARY = -41;

    [NativeTypeName("#define LIBSSH2_ERROR_AGENT_PROTOCOL -42")]
    public const int LIBSSH2_ERROR_AGENT_PROTOCOL = -42;

    [NativeTypeName("#define LIBSSH2_ERROR_SOCKET_RECV -43")]
    public const int LIBSSH2_ERROR_SOCKET_RECV = -43;

    [NativeTypeName("#define LIBSSH2_ERROR_ENCRYPT -44")]
    public const int LIBSSH2_ERROR_ENCRYPT = -44;

    [NativeTypeName("#define LIBSSH2_ERROR_BAD_SOCKET -45")]
    public const int LIBSSH2_ERROR_BAD_SOCKET = -45;

    [NativeTypeName("#define LIBSSH2_ERROR_KNOWN_HOSTS -46")]
    public const int LIBSSH2_ERROR_KNOWN_HOSTS = -46;

    [NativeTypeName("#define LIBSSH2_ERROR_CHANNEL_WINDOW_FULL -47")]
    public const int LIBSSH2_ERROR_CHANNEL_WINDOW_FULL = -47;

    [NativeTypeName("#define LIBSSH2_ERROR_KEYFILE_AUTH_FAILED -48")]
    public const int LIBSSH2_ERROR_KEYFILE_AUTH_FAILED = -48;

    [NativeTypeName("#define LIBSSH2_ERROR_RANDGEN -49")]
    public const int LIBSSH2_ERROR_RANDGEN = -49;

    [NativeTypeName("#define LIBSSH2_ERROR_MISSING_USERAUTH_BANNER -50")]
    public const int LIBSSH2_ERROR_MISSING_USERAUTH_BANNER = -50;

    [NativeTypeName("#define LIBSSH2_ERROR_ALGO_UNSUPPORTED -51")]
    public const int LIBSSH2_ERROR_ALGO_UNSUPPORTED = -51;

    [NativeTypeName("#define LIBSSH2_ERROR_MAC_FAILURE -52")]
    public const int LIBSSH2_ERROR_MAC_FAILURE = -52;

    [NativeTypeName("#define LIBSSH2_ERROR_HASH_INIT -53")]
    public const int LIBSSH2_ERROR_HASH_INIT = -53;

    [NativeTypeName("#define LIBSSH2_ERROR_HASH_CALC -54")]
    public const int LIBSSH2_ERROR_HASH_CALC = -54;

    [NativeTypeName("#define LIBSSH2_ERROR_BANNER_NONE LIBSSH2_ERROR_BANNER_RECV")]
    public const int LIBSSH2_ERROR_BANNER_NONE = -2;

    [NativeTypeName("#define LIBSSH2_INIT_NO_CRYPTO 0x0001")]
    public const int LIBSSH2_INIT_NO_CRYPTO = 0x0001;

    [NativeTypeName("#define LIBSSH2_CHANNEL_WINDOW_DEFAULT (2*1024*1024)")]
    public const int LIBSSH2_CHANNEL_WINDOW_DEFAULT = (2 * 1024 * 1024);

    [NativeTypeName("#define LIBSSH2_CHANNEL_PACKET_DEFAULT 32768")]
    public const int LIBSSH2_CHANNEL_PACKET_DEFAULT = 32768;

    [NativeTypeName("#define LIBSSH2_CHANNEL_MINADJUST 1024")]
    public const int LIBSSH2_CHANNEL_MINADJUST = 1024;

    [NativeTypeName("#define LIBSSH2_CHANNEL_EXTENDED_DATA_NORMAL 0")]
    public const int LIBSSH2_CHANNEL_EXTENDED_DATA_NORMAL = 0;

    [NativeTypeName("#define LIBSSH2_CHANNEL_EXTENDED_DATA_IGNORE 1")]
    public const int LIBSSH2_CHANNEL_EXTENDED_DATA_IGNORE = 1;

    [NativeTypeName("#define LIBSSH2_CHANNEL_EXTENDED_DATA_MERGE 2")]
    public const int LIBSSH2_CHANNEL_EXTENDED_DATA_MERGE = 2;

    [NativeTypeName("#define SSH_EXTENDED_DATA_STDERR 1")]
    public const int SSH_EXTENDED_DATA_STDERR = 1;

    [NativeTypeName("#define LIBSSH2CHANNEL_EAGAIN LIBSSH2_ERROR_EAGAIN")]
    public const int LIBSSH2CHANNEL_EAGAIN = -37;

    [NativeTypeName("#define LIBSSH2_CHANNEL_FLUSH_EXTENDED_DATA -1")]
    public const int LIBSSH2_CHANNEL_FLUSH_EXTENDED_DATA = -1;

    [NativeTypeName("#define LIBSSH2_CHANNEL_FLUSH_ALL -2")]
    public const int LIBSSH2_CHANNEL_FLUSH_ALL = -2;

    [NativeTypeName("#define HAVE_LIBSSH2_KNOWNHOST_API 0x010101")]
    public const int HAVE_LIBSSH2_KNOWNHOST_API = 0x010101;

    [NativeTypeName("#define HAVE_LIBSSH2_VERSION_API 0x010100")]
    public const int HAVE_LIBSSH2_VERSION_API = 0x010100;

    [NativeTypeName("#define HAVE_LIBSSH2_CRYPTOENGINE_API 0x011100")]
    public const int HAVE_LIBSSH2_CRYPTOENGINE_API = 0x011100;

    [NativeTypeName("#define LIBSSH2_KNOWNHOST_TYPE_MASK 0xffff")]
    public const int LIBSSH2_KNOWNHOST_TYPE_MASK = 0xffff;

    [NativeTypeName("#define LIBSSH2_KNOWNHOST_TYPE_PLAIN 1")]
    public const int LIBSSH2_KNOWNHOST_TYPE_PLAIN = 1;

    [NativeTypeName("#define LIBSSH2_KNOWNHOST_TYPE_SHA1 2")]
    public const int LIBSSH2_KNOWNHOST_TYPE_SHA1 = 2;

    [NativeTypeName("#define LIBSSH2_KNOWNHOST_TYPE_CUSTOM 3")]
    public const int LIBSSH2_KNOWNHOST_TYPE_CUSTOM = 3;

    [NativeTypeName("#define LIBSSH2_KNOWNHOST_KEYENC_MASK (3<<16)")]
    public const int LIBSSH2_KNOWNHOST_KEYENC_MASK = (3 << 16);

    [NativeTypeName("#define LIBSSH2_KNOWNHOST_KEYENC_RAW (1<<16)")]
    public const int LIBSSH2_KNOWNHOST_KEYENC_RAW = (1 << 16);

    [NativeTypeName("#define LIBSSH2_KNOWNHOST_KEYENC_BASE64 (2<<16)")]
    public const int LIBSSH2_KNOWNHOST_KEYENC_BASE64 = (2 << 16);

    [NativeTypeName("#define LIBSSH2_KNOWNHOST_KEY_MASK (15<<18)")]
    public const int LIBSSH2_KNOWNHOST_KEY_MASK = (15 << 18);

    [NativeTypeName("#define LIBSSH2_KNOWNHOST_KEY_SHIFT 18")]
    public const int LIBSSH2_KNOWNHOST_KEY_SHIFT = 18;

    [NativeTypeName("#define LIBSSH2_KNOWNHOST_KEY_RSA1 (1<<18)")]
    public const int LIBSSH2_KNOWNHOST_KEY_RSA1 = (1 << 18);

    [NativeTypeName("#define LIBSSH2_KNOWNHOST_KEY_SSHRSA (2<<18)")]
    public const int LIBSSH2_KNOWNHOST_KEY_SSHRSA = (2 << 18);

    [NativeTypeName("#define LIBSSH2_KNOWNHOST_KEY_SSHDSS (3<<18)")]
    public const int LIBSSH2_KNOWNHOST_KEY_SSHDSS = (3 << 18);

    [NativeTypeName("#define LIBSSH2_KNOWNHOST_KEY_ECDSA_256 (4<<18)")]
    public const int LIBSSH2_KNOWNHOST_KEY_ECDSA_256 = (4 << 18);

    [NativeTypeName("#define LIBSSH2_KNOWNHOST_KEY_ECDSA_384 (5<<18)")]
    public const int LIBSSH2_KNOWNHOST_KEY_ECDSA_384 = (5 << 18);

    [NativeTypeName("#define LIBSSH2_KNOWNHOST_KEY_ECDSA_521 (6<<18)")]
    public const int LIBSSH2_KNOWNHOST_KEY_ECDSA_521 = (6 << 18);

    [NativeTypeName("#define LIBSSH2_KNOWNHOST_KEY_ED25519 (7<<18)")]
    public const int LIBSSH2_KNOWNHOST_KEY_ED25519 = (7 << 18);

    [NativeTypeName("#define LIBSSH2_KNOWNHOST_KEY_UNKNOWN (15<<18)")]
    public const int LIBSSH2_KNOWNHOST_KEY_UNKNOWN = (15 << 18);

    [NativeTypeName("#define LIBSSH2_KNOWNHOST_CHECK_MATCH 0")]
    public const int LIBSSH2_KNOWNHOST_CHECK_MATCH = 0;

    [NativeTypeName("#define LIBSSH2_KNOWNHOST_CHECK_MISMATCH 1")]
    public const int LIBSSH2_KNOWNHOST_CHECK_MISMATCH = 1;

    [NativeTypeName("#define LIBSSH2_KNOWNHOST_CHECK_NOTFOUND 2")]
    public const int LIBSSH2_KNOWNHOST_CHECK_NOTFOUND = 2;

    [NativeTypeName("#define LIBSSH2_KNOWNHOST_CHECK_FAILURE 3")]
    public const int LIBSSH2_KNOWNHOST_CHECK_FAILURE = 3;

    [NativeTypeName("#define LIBSSH2_KNOWNHOST_FILE_OPENSSH 1")]
    public const int LIBSSH2_KNOWNHOST_FILE_OPENSSH = 1;

    [NativeTypeName("#define HAVE_LIBSSH2_AGENT_API 0x010202")]
    public const int HAVE_LIBSSH2_AGENT_API = 0x010202;

    [NativeTypeName("#define LIBSSH2_TRACE_TRANS (1<<1)")]
    public const int LIBSSH2_TRACE_TRANS = (1 << 1);

    [NativeTypeName("#define LIBSSH2_TRACE_KEX (1<<2)")]
    public const int LIBSSH2_TRACE_KEX = (1 << 2);

    [NativeTypeName("#define LIBSSH2_TRACE_AUTH (1<<3)")]
    public const int LIBSSH2_TRACE_AUTH = (1 << 3);

    [NativeTypeName("#define LIBSSH2_TRACE_CONN (1<<4)")]
    public const int LIBSSH2_TRACE_CONN = (1 << 4);

    [NativeTypeName("#define LIBSSH2_TRACE_SCP (1<<5)")]
    public const int LIBSSH2_TRACE_SCP = (1 << 5);

    [NativeTypeName("#define LIBSSH2_TRACE_SFTP (1<<6)")]
    public const int LIBSSH2_TRACE_SFTP = (1 << 6);

    [NativeTypeName("#define LIBSSH2_TRACE_ERROR (1<<7)")]
    public const int LIBSSH2_TRACE_ERROR = (1 << 7);

    [NativeTypeName("#define LIBSSH2_TRACE_PUBLICKEY (1<<8)")]
    public const int LIBSSH2_TRACE_PUBLICKEY = (1 << 8);

    [NativeTypeName("#define LIBSSH2_TRACE_SOCKET (1<<9)")]
    public const int LIBSSH2_TRACE_SOCKET = (1 << 9);

    [DllImport("libssh2", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
    [return: NativeTypeName("LIBSSH2_PUBLICKEY *")]
    public static extern _LIBSSH2_PUBLICKEY* libssh2_publickey_init([NativeTypeName("LIBSSH2_SESSION *")] _LIBSSH2_SESSION* session);

    [DllImport("libssh2", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
    public static extern int libssh2_publickey_add_ex([NativeTypeName("LIBSSH2_PUBLICKEY *")] _LIBSSH2_PUBLICKEY* pkey, [NativeTypeName("const unsigned char *")] byte* name, [NativeTypeName("unsigned long")] uint name_len, [NativeTypeName("const unsigned char *")] byte* blob, [NativeTypeName("unsigned long")] uint blob_len, [NativeTypeName("char")] sbyte overwrite, [NativeTypeName("unsigned long")] uint num_attrs, [NativeTypeName("const libssh2_publickey_attribute[]")] _libssh2_publickey_attribute* attrs);

    [DllImport("libssh2", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
    public static extern int libssh2_publickey_remove_ex([NativeTypeName("LIBSSH2_PUBLICKEY *")] _LIBSSH2_PUBLICKEY* pkey, [NativeTypeName("const unsigned char *")] byte* name, [NativeTypeName("unsigned long")] uint name_len, [NativeTypeName("const unsigned char *")] byte* blob, [NativeTypeName("unsigned long")] uint blob_len);

    [DllImport("libssh2", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
    public static extern int libssh2_publickey_list_fetch([NativeTypeName("LIBSSH2_PUBLICKEY *")] _LIBSSH2_PUBLICKEY* pkey, [NativeTypeName("unsigned long *")] uint* num_keys, [NativeTypeName("libssh2_publickey_list **")] _libssh2_publickey_list** pkey_list);

    [DllImport("libssh2", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
    public static extern void libssh2_publickey_list_free([NativeTypeName("LIBSSH2_PUBLICKEY *")] _LIBSSH2_PUBLICKEY* pkey, [NativeTypeName("libssh2_publickey_list *")] _libssh2_publickey_list* pkey_list);

    [DllImport("libssh2", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
    public static extern int libssh2_publickey_shutdown([NativeTypeName("LIBSSH2_PUBLICKEY *")] _LIBSSH2_PUBLICKEY* pkey);

    [NativeTypeName("#define LIBSSH2_PUBLICKEY_H 1")]
    public const int LIBSSH2_PUBLICKEY_H = 1;

    [DllImport("libssh2", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
    [return: NativeTypeName("LIBSSH2_SFTP *")]
    public static extern _LIBSSH2_SFTP* libssh2_sftp_init([NativeTypeName("LIBSSH2_SESSION *")] _LIBSSH2_SESSION* session);

    [DllImport("libssh2", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
    public static extern int libssh2_sftp_shutdown([NativeTypeName("LIBSSH2_SFTP *")] _LIBSSH2_SFTP* sftp);

    [DllImport("libssh2", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
    [return: NativeTypeName("unsigned long")]
    public static extern uint libssh2_sftp_last_error([NativeTypeName("LIBSSH2_SFTP *")] _LIBSSH2_SFTP* sftp);

    [DllImport("libssh2", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
    [return: NativeTypeName("LIBSSH2_CHANNEL *")]
    public static extern _LIBSSH2_CHANNEL* libssh2_sftp_get_channel([NativeTypeName("LIBSSH2_SFTP *")] _LIBSSH2_SFTP* sftp);

    [DllImport("libssh2", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
    [return: NativeTypeName("LIBSSH2_SFTP_HANDLE *")]
    public static extern _LIBSSH2_SFTP_HANDLE* libssh2_sftp_open_ex([NativeTypeName("LIBSSH2_SFTP *")] _LIBSSH2_SFTP* sftp, [NativeTypeName("const char *")] sbyte* filename, [NativeTypeName("unsigned int")] uint filename_len, [NativeTypeName("unsigned long")] uint flags, [NativeTypeName("long")] int mode, int open_type);

    [DllImport("libssh2", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
    [return: NativeTypeName("LIBSSH2_SFTP_HANDLE *")]
    public static extern _LIBSSH2_SFTP_HANDLE* libssh2_sftp_open_ex_r([NativeTypeName("LIBSSH2_SFTP *")] _LIBSSH2_SFTP* sftp, [NativeTypeName("const char *")] sbyte* filename, [NativeTypeName("size_t")] nuint filename_len, [NativeTypeName("unsigned long")] uint flags, [NativeTypeName("long")] int mode, int open_type, [NativeTypeName("LIBSSH2_SFTP_ATTRIBUTES *")] _LIBSSH2_SFTP_ATTRIBUTES* attrs);

    [DllImport("libssh2", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
    [return: NativeTypeName("ssize_t")]
    public static extern long libssh2_sftp_read([NativeTypeName("LIBSSH2_SFTP_HANDLE *")] _LIBSSH2_SFTP_HANDLE* handle, [NativeTypeName("char *")] sbyte* buffer, [NativeTypeName("size_t")] nuint buffer_maxlen);

    [DllImport("libssh2", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
    public static extern int libssh2_sftp_readdir_ex([NativeTypeName("LIBSSH2_SFTP_HANDLE *")] _LIBSSH2_SFTP_HANDLE* handle, [NativeTypeName("char *")] sbyte* buffer, [NativeTypeName("size_t")] nuint buffer_maxlen, [NativeTypeName("char *")] sbyte* longentry, [NativeTypeName("size_t")] nuint longentry_maxlen, [NativeTypeName("LIBSSH2_SFTP_ATTRIBUTES *")] _LIBSSH2_SFTP_ATTRIBUTES* attrs);

    [DllImport("libssh2", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
    [return: NativeTypeName("ssize_t")]
    public static extern long libssh2_sftp_write([NativeTypeName("LIBSSH2_SFTP_HANDLE *")] _LIBSSH2_SFTP_HANDLE* handle, [NativeTypeName("const char *")] sbyte* buffer, [NativeTypeName("size_t")] nuint count);

    [DllImport("libssh2", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
    public static extern int libssh2_sftp_fsync([NativeTypeName("LIBSSH2_SFTP_HANDLE *")] _LIBSSH2_SFTP_HANDLE* handle);

    [DllImport("libssh2", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
    public static extern int libssh2_sftp_close_handle([NativeTypeName("LIBSSH2_SFTP_HANDLE *")] _LIBSSH2_SFTP_HANDLE* handle);

    [DllImport("libssh2", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
    public static extern void libssh2_sftp_seek([NativeTypeName("LIBSSH2_SFTP_HANDLE *")] _LIBSSH2_SFTP_HANDLE* handle, [NativeTypeName("size_t")] nuint offset);

    [DllImport("libssh2", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
    public static extern void libssh2_sftp_seek64([NativeTypeName("LIBSSH2_SFTP_HANDLE *")] _LIBSSH2_SFTP_HANDLE* handle, [NativeTypeName("libssh2_uint64_t")] ulong offset);

    [DllImport("libssh2", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
    [return: NativeTypeName("size_t")]
    public static extern nuint libssh2_sftp_tell([NativeTypeName("LIBSSH2_SFTP_HANDLE *")] _LIBSSH2_SFTP_HANDLE* handle);

    [DllImport("libssh2", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
    [return: NativeTypeName("libssh2_uint64_t")]
    public static extern ulong libssh2_sftp_tell64([NativeTypeName("LIBSSH2_SFTP_HANDLE *")] _LIBSSH2_SFTP_HANDLE* handle);

    [DllImport("libssh2", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
    public static extern int libssh2_sftp_fstat_ex([NativeTypeName("LIBSSH2_SFTP_HANDLE *")] _LIBSSH2_SFTP_HANDLE* handle, [NativeTypeName("LIBSSH2_SFTP_ATTRIBUTES *")] _LIBSSH2_SFTP_ATTRIBUTES* attrs, int setstat);

    [DllImport("libssh2", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
    public static extern int libssh2_sftp_rename_ex([NativeTypeName("LIBSSH2_SFTP *")] _LIBSSH2_SFTP* sftp, [NativeTypeName("const char *")] sbyte* source_filename, [NativeTypeName("unsigned int")] uint srouce_filename_len, [NativeTypeName("const char *")] sbyte* dest_filename, [NativeTypeName("unsigned int")] uint dest_filename_len, [NativeTypeName("long")] int flags);

    [DllImport("libssh2", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
    public static extern int libssh2_sftp_posix_rename_ex([NativeTypeName("LIBSSH2_SFTP *")] _LIBSSH2_SFTP* sftp, [NativeTypeName("const char *")] sbyte* source_filename, [NativeTypeName("size_t")] nuint srouce_filename_len, [NativeTypeName("const char *")] sbyte* dest_filename, [NativeTypeName("size_t")] nuint dest_filename_len);

    [DllImport("libssh2", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
    public static extern int libssh2_sftp_unlink_ex([NativeTypeName("LIBSSH2_SFTP *")] _LIBSSH2_SFTP* sftp, [NativeTypeName("const char *")] sbyte* filename, [NativeTypeName("unsigned int")] uint filename_len);

    [DllImport("libssh2", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
    public static extern int libssh2_sftp_fstatvfs([NativeTypeName("LIBSSH2_SFTP_HANDLE *")] _LIBSSH2_SFTP_HANDLE* handle, [NativeTypeName("LIBSSH2_SFTP_STATVFS *")] _LIBSSH2_SFTP_STATVFS* st);

    [DllImport("libssh2", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
    public static extern int libssh2_sftp_statvfs([NativeTypeName("LIBSSH2_SFTP *")] _LIBSSH2_SFTP* sftp, [NativeTypeName("const char *")] sbyte* path, [NativeTypeName("size_t")] nuint path_len, [NativeTypeName("LIBSSH2_SFTP_STATVFS *")] _LIBSSH2_SFTP_STATVFS* st);

    [DllImport("libssh2", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
    public static extern int libssh2_sftp_mkdir_ex([NativeTypeName("LIBSSH2_SFTP *")] _LIBSSH2_SFTP* sftp, [NativeTypeName("const char *")] sbyte* path, [NativeTypeName("unsigned int")] uint path_len, [NativeTypeName("long")] int mode);

    [DllImport("libssh2", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
    public static extern int libssh2_sftp_rmdir_ex([NativeTypeName("LIBSSH2_SFTP *")] _LIBSSH2_SFTP* sftp, [NativeTypeName("const char *")] sbyte* path, [NativeTypeName("unsigned int")] uint path_len);

    [DllImport("libssh2", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
    public static extern int libssh2_sftp_stat_ex([NativeTypeName("LIBSSH2_SFTP *")] _LIBSSH2_SFTP* sftp, [NativeTypeName("const char *")] sbyte* path, [NativeTypeName("unsigned int")] uint path_len, int stat_type, [NativeTypeName("LIBSSH2_SFTP_ATTRIBUTES *")] _LIBSSH2_SFTP_ATTRIBUTES* attrs);

    [DllImport("libssh2", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
    public static extern int libssh2_sftp_symlink_ex([NativeTypeName("LIBSSH2_SFTP *")] _LIBSSH2_SFTP* sftp, [NativeTypeName("const char *")] sbyte* path, [NativeTypeName("unsigned int")] uint path_len, [NativeTypeName("char *")] sbyte* target, [NativeTypeName("unsigned int")] uint target_len, int link_type);

    [NativeTypeName("#define LIBSSH2_SFTP_H 1")]
    public const int LIBSSH2_SFTP_H = 1;

    [NativeTypeName("#define LIBSSH2_SFTP_VERSION 3")]
    public const int LIBSSH2_SFTP_VERSION = 3;

    [NativeTypeName("#define LIBSSH2_SFTP_OPENFILE 0")]
    public const int LIBSSH2_SFTP_OPENFILE = 0;

    [NativeTypeName("#define LIBSSH2_SFTP_OPENDIR 1")]
    public const int LIBSSH2_SFTP_OPENDIR = 1;

    [NativeTypeName("#define LIBSSH2_SFTP_RENAME_OVERWRITE 0x00000001")]
    public const int LIBSSH2_SFTP_RENAME_OVERWRITE = 0x00000001;

    [NativeTypeName("#define LIBSSH2_SFTP_RENAME_ATOMIC 0x00000002")]
    public const int LIBSSH2_SFTP_RENAME_ATOMIC = 0x00000002;

    [NativeTypeName("#define LIBSSH2_SFTP_RENAME_NATIVE 0x00000004")]
    public const int LIBSSH2_SFTP_RENAME_NATIVE = 0x00000004;

    [NativeTypeName("#define LIBSSH2_SFTP_STAT 0")]
    public const int LIBSSH2_SFTP_STAT = 0;

    [NativeTypeName("#define LIBSSH2_SFTP_LSTAT 1")]
    public const int LIBSSH2_SFTP_LSTAT = 1;

    [NativeTypeName("#define LIBSSH2_SFTP_SETSTAT 2")]
    public const int LIBSSH2_SFTP_SETSTAT = 2;

    [NativeTypeName("#define LIBSSH2_SFTP_SYMLINK 0")]
    public const int LIBSSH2_SFTP_SYMLINK = 0;

    [NativeTypeName("#define LIBSSH2_SFTP_READLINK 1")]
    public const int LIBSSH2_SFTP_READLINK = 1;

    [NativeTypeName("#define LIBSSH2_SFTP_REALPATH 2")]
    public const int LIBSSH2_SFTP_REALPATH = 2;

    [NativeTypeName("#define LIBSSH2_SFTP_DEFAULT_MODE -1")]
    public const int LIBSSH2_SFTP_DEFAULT_MODE = -1;

    [NativeTypeName("#define LIBSSH2_SFTP_ATTR_SIZE 0x00000001")]
    public const int LIBSSH2_SFTP_ATTR_SIZE = 0x00000001;

    [NativeTypeName("#define LIBSSH2_SFTP_ATTR_UIDGID 0x00000002")]
    public const int LIBSSH2_SFTP_ATTR_UIDGID = 0x00000002;

    [NativeTypeName("#define LIBSSH2_SFTP_ATTR_PERMISSIONS 0x00000004")]
    public const int LIBSSH2_SFTP_ATTR_PERMISSIONS = 0x00000004;

    [NativeTypeName("#define LIBSSH2_SFTP_ATTR_ACMODTIME 0x00000008")]
    public const int LIBSSH2_SFTP_ATTR_ACMODTIME = 0x00000008;

    [NativeTypeName("#define LIBSSH2_SFTP_ATTR_EXTENDED 0x80000000")]
    public const uint LIBSSH2_SFTP_ATTR_EXTENDED = 0x80000000;

    [NativeTypeName("#define LIBSSH2_SFTP_ST_RDONLY 0x00000001")]
    public const int LIBSSH2_SFTP_ST_RDONLY = 0x00000001;

    [NativeTypeName("#define LIBSSH2_SFTP_ST_NOSUID 0x00000002")]
    public const int LIBSSH2_SFTP_ST_NOSUID = 0x00000002;

    [NativeTypeName("#define LIBSSH2_SFTP_TYPE_REGULAR 1")]
    public const int LIBSSH2_SFTP_TYPE_REGULAR = 1;

    [NativeTypeName("#define LIBSSH2_SFTP_TYPE_DIRECTORY 2")]
    public const int LIBSSH2_SFTP_TYPE_DIRECTORY = 2;

    [NativeTypeName("#define LIBSSH2_SFTP_TYPE_SYMLINK 3")]
    public const int LIBSSH2_SFTP_TYPE_SYMLINK = 3;

    [NativeTypeName("#define LIBSSH2_SFTP_TYPE_SPECIAL 4")]
    public const int LIBSSH2_SFTP_TYPE_SPECIAL = 4;

    [NativeTypeName("#define LIBSSH2_SFTP_TYPE_UNKNOWN 5")]
    public const int LIBSSH2_SFTP_TYPE_UNKNOWN = 5;

    [NativeTypeName("#define LIBSSH2_SFTP_TYPE_SOCKET 6")]
    public const int LIBSSH2_SFTP_TYPE_SOCKET = 6;

    [NativeTypeName("#define LIBSSH2_SFTP_TYPE_CHAR_DEVICE 7")]
    public const int LIBSSH2_SFTP_TYPE_CHAR_DEVICE = 7;

    [NativeTypeName("#define LIBSSH2_SFTP_TYPE_BLOCK_DEVICE 8")]
    public const int LIBSSH2_SFTP_TYPE_BLOCK_DEVICE = 8;

    [NativeTypeName("#define LIBSSH2_SFTP_TYPE_FIFO 9")]
    public const int LIBSSH2_SFTP_TYPE_FIFO = 9;

    [NativeTypeName("#define LIBSSH2_SFTP_S_IFMT 0170000")]
    public const int LIBSSH2_SFTP_S_IFMT = 0170000;

    [NativeTypeName("#define LIBSSH2_SFTP_S_IFIFO 0010000")]
    public const int LIBSSH2_SFTP_S_IFIFO = 0010000;

    [NativeTypeName("#define LIBSSH2_SFTP_S_IFCHR 0020000")]
    public const int LIBSSH2_SFTP_S_IFCHR = 0020000;

    [NativeTypeName("#define LIBSSH2_SFTP_S_IFDIR 0040000")]
    public const int LIBSSH2_SFTP_S_IFDIR = 0040000;

    [NativeTypeName("#define LIBSSH2_SFTP_S_IFBLK 0060000")]
    public const int LIBSSH2_SFTP_S_IFBLK = 0060000;

    [NativeTypeName("#define LIBSSH2_SFTP_S_IFREG 0100000")]
    public const int LIBSSH2_SFTP_S_IFREG = 0100000;

    [NativeTypeName("#define LIBSSH2_SFTP_S_IFLNK 0120000")]
    public const int LIBSSH2_SFTP_S_IFLNK = 0120000;

    [NativeTypeName("#define LIBSSH2_SFTP_S_IFSOCK 0140000")]
    public const int LIBSSH2_SFTP_S_IFSOCK = 0140000;

    [NativeTypeName("#define LIBSSH2_SFTP_S_IRWXU 0000700")]
    public const int LIBSSH2_SFTP_S_IRWXU = 0000700;

    [NativeTypeName("#define LIBSSH2_SFTP_S_IRUSR 0000400")]
    public const int LIBSSH2_SFTP_S_IRUSR = 0000400;

    [NativeTypeName("#define LIBSSH2_SFTP_S_IWUSR 0000200")]
    public const int LIBSSH2_SFTP_S_IWUSR = 0000200;

    [NativeTypeName("#define LIBSSH2_SFTP_S_IXUSR 0000100")]
    public const int LIBSSH2_SFTP_S_IXUSR = 0000100;

    [NativeTypeName("#define LIBSSH2_SFTP_S_IRWXG 0000070")]
    public const int LIBSSH2_SFTP_S_IRWXG = 0000070;

    [NativeTypeName("#define LIBSSH2_SFTP_S_IRGRP 0000040")]
    public const int LIBSSH2_SFTP_S_IRGRP = 0000040;

    [NativeTypeName("#define LIBSSH2_SFTP_S_IWGRP 0000020")]
    public const int LIBSSH2_SFTP_S_IWGRP = 0000020;

    [NativeTypeName("#define LIBSSH2_SFTP_S_IXGRP 0000010")]
    public const int LIBSSH2_SFTP_S_IXGRP = 0000010;

    [NativeTypeName("#define LIBSSH2_SFTP_S_IRWXO 0000007")]
    public const int LIBSSH2_SFTP_S_IRWXO = 0000007;

    [NativeTypeName("#define LIBSSH2_SFTP_S_IROTH 0000004")]
    public const int LIBSSH2_SFTP_S_IROTH = 0000004;

    [NativeTypeName("#define LIBSSH2_SFTP_S_IWOTH 0000002")]
    public const int LIBSSH2_SFTP_S_IWOTH = 0000002;

    [NativeTypeName("#define LIBSSH2_SFTP_S_IXOTH 0000001")]
    public const int LIBSSH2_SFTP_S_IXOTH = 0000001;

    [NativeTypeName("#define LIBSSH2_FXF_READ 0x00000001")]
    public const int LIBSSH2_FXF_READ = 0x00000001;

    [NativeTypeName("#define LIBSSH2_FXF_WRITE 0x00000002")]
    public const int LIBSSH2_FXF_WRITE = 0x00000002;

    [NativeTypeName("#define LIBSSH2_FXF_APPEND 0x00000004")]
    public const int LIBSSH2_FXF_APPEND = 0x00000004;

    [NativeTypeName("#define LIBSSH2_FXF_CREAT 0x00000008")]
    public const int LIBSSH2_FXF_CREAT = 0x00000008;

    [NativeTypeName("#define LIBSSH2_FXF_TRUNC 0x00000010")]
    public const int LIBSSH2_FXF_TRUNC = 0x00000010;

    [NativeTypeName("#define LIBSSH2_FXF_EXCL 0x00000020")]
    public const int LIBSSH2_FXF_EXCL = 0x00000020;

    [NativeTypeName("#define LIBSSH2_FX_OK 0UL")]
    public const uint LIBSSH2_FX_OK = 0U;

    [NativeTypeName("#define LIBSSH2_FX_EOF 1UL")]
    public const uint LIBSSH2_FX_EOF = 1U;

    [NativeTypeName("#define LIBSSH2_FX_NO_SUCH_FILE 2UL")]
    public const uint LIBSSH2_FX_NO_SUCH_FILE = 2U;

    [NativeTypeName("#define LIBSSH2_FX_PERMISSION_DENIED 3UL")]
    public const uint LIBSSH2_FX_PERMISSION_DENIED = 3U;

    [NativeTypeName("#define LIBSSH2_FX_FAILURE 4UL")]
    public const uint LIBSSH2_FX_FAILURE = 4U;

    [NativeTypeName("#define LIBSSH2_FX_BAD_MESSAGE 5UL")]
    public const uint LIBSSH2_FX_BAD_MESSAGE = 5U;

    [NativeTypeName("#define LIBSSH2_FX_NO_CONNECTION 6UL")]
    public const uint LIBSSH2_FX_NO_CONNECTION = 6U;

    [NativeTypeName("#define LIBSSH2_FX_CONNECTION_LOST 7UL")]
    public const uint LIBSSH2_FX_CONNECTION_LOST = 7U;

    [NativeTypeName("#define LIBSSH2_FX_OP_UNSUPPORTED 8UL")]
    public const uint LIBSSH2_FX_OP_UNSUPPORTED = 8U;

    [NativeTypeName("#define LIBSSH2_FX_INVALID_HANDLE 9UL")]
    public const uint LIBSSH2_FX_INVALID_HANDLE = 9U;

    [NativeTypeName("#define LIBSSH2_FX_NO_SUCH_PATH 10UL")]
    public const uint LIBSSH2_FX_NO_SUCH_PATH = 10U;

    [NativeTypeName("#define LIBSSH2_FX_FILE_ALREADY_EXISTS 11UL")]
    public const uint LIBSSH2_FX_FILE_ALREADY_EXISTS = 11U;

    [NativeTypeName("#define LIBSSH2_FX_WRITE_PROTECT 12UL")]
    public const uint LIBSSH2_FX_WRITE_PROTECT = 12U;

    [NativeTypeName("#define LIBSSH2_FX_NO_MEDIA 13UL")]
    public const uint LIBSSH2_FX_NO_MEDIA = 13U;

    [NativeTypeName("#define LIBSSH2_FX_NO_SPACE_ON_FILESYSTEM 14UL")]
    public const uint LIBSSH2_FX_NO_SPACE_ON_FILESYSTEM = 14U;

    [NativeTypeName("#define LIBSSH2_FX_QUOTA_EXCEEDED 15UL")]
    public const uint LIBSSH2_FX_QUOTA_EXCEEDED = 15U;

    [NativeTypeName("#define LIBSSH2_FX_UNKNOWN_PRINCIPLE 16UL")]
    public const uint LIBSSH2_FX_UNKNOWN_PRINCIPLE = 16U;

    [NativeTypeName("#define LIBSSH2_FX_UNKNOWN_PRINCIPAL 16UL")]
    public const uint LIBSSH2_FX_UNKNOWN_PRINCIPAL = 16U;

    [NativeTypeName("#define LIBSSH2_FX_LOCK_CONFlICT 17UL")]
    public const uint LIBSSH2_FX_LOCK_CONFlICT = 17U;

    [NativeTypeName("#define LIBSSH2_FX_LOCK_CONFLICT 17UL")]
    public const uint LIBSSH2_FX_LOCK_CONFLICT = 17U;

    [NativeTypeName("#define LIBSSH2_FX_DIR_NOT_EMPTY 18UL")]
    public const uint LIBSSH2_FX_DIR_NOT_EMPTY = 18U;

    [NativeTypeName("#define LIBSSH2_FX_NOT_A_DIRECTORY 19UL")]
    public const uint LIBSSH2_FX_NOT_A_DIRECTORY = 19U;

    [NativeTypeName("#define LIBSSH2_FX_INVALID_FILENAME 20UL")]
    public const uint LIBSSH2_FX_INVALID_FILENAME = 20U;

    [NativeTypeName("#define LIBSSH2_FX_LINK_LOOP 21UL")]
    public const uint LIBSSH2_FX_LINK_LOOP = 21U;

    [NativeTypeName("#define LIBSSH2SFTP_EAGAIN LIBSSH2_ERROR_EAGAIN")]
    public const int LIBSSH2SFTP_EAGAIN = -37;
}
