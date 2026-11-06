/* =====================================================================
   UMS CORE JS — Tương tác dùng chung cho Admin, Student và Login
   - Sidebar thu gọn / mở rộng (desktop) và off-canvas (mobile)
   - Toast notification (tích hợp TempData qua [data-flash])
   - Modal xác nhận cho thao tác quan trọng ([data-confirm-trigger])
   - Trạng thái loading + chống gửi form liên tục
   - Tooltip, count-up, phân trang & lọc bảng phía client
   - Tôn trọng prefers-reduced-motion
   Không phụ thuộc jQuery. Cần Bootstrap 5 bundle (Modal, Tooltip).
   ===================================================================== */
(function (window, document) {
    'use strict';

    var root = document.documentElement;
    var reduceMotion = window.matchMedia && window.matchMedia('(prefers-reduced-motion: reduce)').matches;
    var DESKTOP = 992;
    var STORAGE_KEY = 'ums-sidebar-collapsed';

    function $(sel, ctx) { return (ctx || document).querySelector(sel); }
    function $all(sel, ctx) { return Array.prototype.slice.call((ctx || document).querySelectorAll(sel)); }
    function isDesktop() { return window.innerWidth >= DESKTOP; }
    function store(key, val) { try { if (val === undefined) { return window.localStorage.getItem(key); } window.localStorage.setItem(key, val); } catch (e) { return null; } return null; }
    function hasBootstrap(name) { return !!(window.bootstrap && window.bootstrap[name]); }

    /* ------------------------------------------------------------------
       1. SIDEBAR
    ------------------------------------------------------------------ */
    function initSidebar() {
        var toggles = $all('[data-sidebar-toggle]');
        if (!toggles.length) { return; }

        function closeMobile() { root.classList.remove('sidebar-open'); }

        toggles.forEach(function (btn) {
            btn.addEventListener('click', function () {
                if (isDesktop()) {
                    var collapsed = root.classList.toggle('sidebar-collapsed');
                    store(STORAGE_KEY, collapsed ? '1' : '0');
                    btn.setAttribute('aria-expanded', String(!collapsed));
                } else {
                    var open = root.classList.toggle('sidebar-open');
                    btn.setAttribute('aria-expanded', String(open));
                }
            });
        });

        $all('[data-sidebar-close]').forEach(function (el) { el.addEventListener('click', closeMobile); });
        document.addEventListener('keydown', function (e) { if (e.key === 'Escape') { closeMobile(); } });
        window.addEventListener('resize', function () {
            if (isDesktop()) { closeMobile(); }
            else { root.classList.remove('sidebar-collapsed'); }
        });
        if (isDesktop() && store(STORAGE_KEY) === '1') { root.classList.add('sidebar-collapsed'); }

        // Tooltip cho icon menu khi sidebar đang thu gọn
        if (hasBootstrap('Tooltip')) {
            $all('.app-sidebar .side-link').forEach(function (link) {
                var label = link.querySelector('.nav-label');
                if (!label) { return; }
                var tip = new window.bootstrap.Tooltip(link, { title: label.textContent.trim(), placement: 'right', trigger: 'hover focus', container: 'body' });
                link.addEventListener('show.bs.tooltip', function (e) {
                    if (!(isDesktop() && root.classList.contains('sidebar-collapsed'))) { e.preventDefault(); }
                });
                link.addEventListener('click', function () { tip.hide(); });
            });
        }
    }

    /* ------------------------------------------------------------------
       2. TOAST
    ------------------------------------------------------------------ */
    var TOAST_ICONS = {
        success: 'fa-solid fa-circle-check',
        error: 'fa-solid fa-circle-xmark',
        warning: 'fa-solid fa-triangle-exclamation',
        info: 'fa-solid fa-circle-info'
    };
    var TOAST_TITLES = { success: 'Thành công', error: 'Thao tác thất bại', warning: 'Lưu ý', info: 'Thông báo' };

    function toastStack() {
        var stack = $('.ums-toast-stack');
        if (!stack) {
            stack = document.createElement('div');
            stack.className = 'ums-toast-stack';
            stack.setAttribute('aria-live', 'polite');
            stack.setAttribute('aria-atomic', 'false');
            document.body.appendChild(stack);
        }
        return stack;
    }

    function removeToast(el) {
        if (!el || el.classList.contains('is-leaving')) { return; }
        el.classList.add('is-leaving');
        var done = function () { if (el.parentNode) { el.parentNode.removeChild(el); } };
        el.addEventListener('animationend', done, { once: true });
        setTimeout(done, 450);
    }

    /**
     * UMS.toast({ type: 'success'|'error'|'warning'|'info', title, message, duration })
     */
    function toast(opts) {
        opts = typeof opts === 'string' ? { message: opts } : (opts || {});
        var type = TOAST_ICONS[opts.type] ? opts.type : 'info';
        var duration = typeof opts.duration === 'number' ? opts.duration : (type === 'error' ? 5000 : 3000);

        var el = document.createElement('div');
        el.className = 'ums-toast toast-' + type;
        el.setAttribute('role', type === 'error' ? 'alert' : 'status');

        var icon = document.createElement('div');
        icon.className = 'toast-icon';
        icon.innerHTML = '<i class="' + TOAST_ICONS[type] + '" aria-hidden="true"></i>';

        var body = document.createElement('div');
        body.className = 'toast-body-wrap';
        var title = document.createElement('div');
        title.className = 'toast-title';
        title.textContent = opts.title || TOAST_TITLES[type];
        body.appendChild(title);
        if (opts.message) {
            var msg = document.createElement('div');
            msg.className = 'toast-msg';
            msg.textContent = opts.message;
            body.appendChild(msg);
        }

        var close = document.createElement('button');
        close.type = 'button';
        close.className = 'toast-close';
        close.setAttribute('aria-label', 'Đóng thông báo');
        close.innerHTML = '<i class="fa-solid fa-xmark" aria-hidden="true"></i>';
        close.addEventListener('click', function () { removeToast(el); });

        el.appendChild(icon);
        el.appendChild(body);
        el.appendChild(close);

        if (duration > 0) {
            var bar = document.createElement('div');
            bar.className = 'toast-progress';
            bar.style.animationDuration = duration + 'ms';
            el.appendChild(bar);
            // Thời gian tự đóng dựa trên thanh tiến trình -> tự tạm dừng khi rê chuột
            bar.addEventListener('animationend', function () { removeToast(el); });
            if (reduceMotion) { setTimeout(function () { removeToast(el); }, duration); }
        }

        toastStack().appendChild(el);
        return el;
    }

    // Chuyển thông báo TempData (render sẵn ở layout) thành toast
    function initFlash() {
        var items = $all('[data-flash]');
        items.forEach(function (item, i) {
            var message = (item.querySelector('.flash-text') || item).textContent.trim();
            var type = item.getAttribute('data-flash');
            var title = item.getAttribute('data-flash-title') || undefined;
            setTimeout(function () { toast({ type: type, title: title, message: message }); }, 150 + i * 180);
            item.parentNode.removeChild(item);
        });
        $all('.flash-fallback').forEach(function (w) { if (!w.children.length) { w.parentNode.removeChild(w); } });
    }

    // Lỗi kết nối / lỗi xử lý cho các request fetch (VD: tải lớp theo khoa)
    function initNetworkFeedback() {
        if (window.fetch && !window.fetch.__ums) {
            var nativeFetch = window.fetch;
            var wrapped = function () {
                return nativeFetch.apply(this, arguments).then(function (res) {
                    if (!res.ok) {
                        toast({ type: 'error', title: 'Lỗi xử lý', message: 'Máy chủ phản hồi lỗi (HTTP ' + res.status + '). Vui lòng thử lại.' });
                    }
                    return res;
                }, function (err) {
                    if (!err || err.name !== 'AbortError') {
                        toast({ type: 'error', title: 'Lỗi kết nối', message: 'Không thể kết nối tới máy chủ. Kiểm tra mạng và thử lại.' });
                    }
                    throw err;
                });
            };
            wrapped.__ums = true;
            window.fetch = wrapped;
        }
        window.addEventListener('offline', function () { toast({ type: 'warning', title: 'Mất kết nối mạng', message: 'Bạn đang ngoại tuyến. Các thao tác có thể không được lưu.' }); });
        window.addEventListener('online', function () { toast({ type: 'success', title: 'Đã kết nối lại', message: 'Kết nối mạng đã được khôi phục.' }); });
    }

    /* ------------------------------------------------------------------
       3. MODAL XÁC NHẬN
    ------------------------------------------------------------------ */
    var confirmModalEl = null;
    var confirmResolve = null;

    function buildConfirmModal() {
        if (confirmModalEl) { return confirmModalEl; }
        var wrap = document.createElement('div');
        wrap.className = 'modal fade modal-confirm';
        wrap.id = 'umsConfirmModal';
        wrap.tabIndex = -1;
        wrap.setAttribute('aria-hidden', 'true');
        wrap.setAttribute('aria-labelledby', 'umsConfirmTitle');
        wrap.innerHTML =
            '<div class="modal-dialog modal-dialog-centered" style="max-width:440px">' +
            '  <div class="modal-content">' +
            '    <div class="modal-body">' +
            '      <div class="confirm-icon"><i class="fa-solid fa-trash-can" aria-hidden="true"></i></div>' +
            '      <h5 class="confirm-title" id="umsConfirmTitle"></h5>' +
            '      <p class="confirm-text"></p>' +
            '      <div class="confirm-target d-none"><i class="fa-solid fa-tag" aria-hidden="true"></i><span></span></div>' +
            '      <p class="confirm-note form-hint mb-0 d-none"></p>' +
            '    </div>' +
            '    <div class="modal-footer">' +
            '      <button type="button" class="btn btn-light" data-bs-dismiss="modal">Hủy</button>' +
            '      <button type="button" class="btn btn-danger" data-confirm-ok><span>Xác nhận</span></button>' +
            '    </div>' +
            '  </div>' +
            '</div>';
        document.body.appendChild(wrap);

        wrap.querySelector('[data-confirm-ok]').addEventListener('click', function () {
            if (confirmResolve) { var r = confirmResolve; confirmResolve = null; r(true, this); }
        });
        wrap.addEventListener('hidden.bs.modal', function () {
            if (confirmResolve) { var r = confirmResolve; confirmResolve = null; r(false); }
            var ok = wrap.querySelector('[data-confirm-ok]');
            ok.classList.remove('is-loading');
            ok.disabled = false;
        });
        confirmModalEl = wrap;
        return wrap;
    }

    /**
     * UMS.confirm({ title, message, name, note, okText, variant: 'danger'|'warning'|'primary', icon })
     * Trả về Promise<boolean>. Nếu không có Bootstrap Modal thì dùng window.confirm.
     */
    function confirmDialog(opts) {
        opts = opts || {};
        if (!hasBootstrap('Modal')) {
            return Promise.resolve(window.confirm((opts.message || 'Bạn có chắc chắn?') + (opts.name ? '\n' + opts.name : '')));
        }
        var el = buildConfirmModal();
        var variant = opts.variant || 'danger';
        el.classList.remove('variant-danger', 'variant-warning', 'variant-primary');
        el.classList.add('variant-' + variant);
        el.querySelector('.confirm-icon i').className = opts.icon || (variant === 'danger' ? 'fa-solid fa-trash-can' : variant === 'warning' ? 'fa-solid fa-triangle-exclamation' : 'fa-solid fa-circle-question');
        el.querySelector('.confirm-title').textContent = opts.title || 'Xác nhận thao tác';
        el.querySelector('.confirm-text').textContent = opts.message || 'Bạn có chắc chắn muốn thực hiện thao tác này?';
        var target = el.querySelector('.confirm-target');
        target.classList.toggle('d-none', !opts.name);
        target.querySelector('span').textContent = opts.name || '';
        var note = el.querySelector('.confirm-note');
        note.classList.toggle('d-none', !opts.note);
        note.textContent = opts.note || '';
        var ok = el.querySelector('[data-confirm-ok]');
        ok.className = 'btn ' + (variant === 'danger' ? 'btn-danger' : variant === 'warning' ? 'btn-warning' : 'btn-primary');
        ok.querySelector('span').textContent = opts.okText || 'Xác nhận';

        var modal = window.bootstrap.Modal.getOrCreateInstance(el);
        return new Promise(function (resolve) {
            confirmResolve = function (result, btn) {
                if (result && opts.keepOpen && btn) {
                    btn.classList.add('is-loading');
                    btn.disabled = true;
                } else {
                    modal.hide();
                }
                resolve(result);
            };
            modal.show();
        });
    }

    function initConfirmTriggers() {
        document.addEventListener('click', function (e) {
            var trigger = e.target.closest('[data-confirm-trigger]');
            if (!trigger) { return; }
            e.preventDefault();
            var formSel = trigger.getAttribute('data-confirm-form');
            var form = formSel ? $(formSel) : trigger.closest('form');
            if (!form) { return; }
            confirmDialog({
                title: trigger.getAttribute('data-confirm-title'),
                message: trigger.getAttribute('data-confirm-message'),
                name: trigger.getAttribute('data-confirm-name'),
                note: trigger.getAttribute('data-confirm-note'),
                okText: trigger.getAttribute('data-confirm-ok'),
                variant: trigger.getAttribute('data-confirm-variant') || 'danger',
                icon: trigger.getAttribute('data-confirm-icon'),
                keepOpen: true
            }).then(function (ok) {
                if (!ok) { return; }
                if (form.getAttribute('data-submitting') === '1') { return; }
                form.setAttribute('data-submitting', '1');
                trigger.classList.add('is-loading');
                startNavProgress();
                // Dùng prototype để tránh trường hợp form có input tên "submit"
                HTMLFormElement.prototype.submit.call(form);
            });
        });
    }

    /* ------------------------------------------------------------------
       4. LOADING KHI GỬI FORM + CHỐNG GỬI LIÊN TỤC
    ------------------------------------------------------------------ */
    function setLoading(btn) {
        if (!btn || btn.classList.contains('is-loading')) { return; }
        btn.classList.add('is-loading');
        btn.setAttribute('aria-busy', 'true');
        if (btn.getAttribute('data-loading-text')) {
            btn.setAttribute('data-original-html', btn.innerHTML);
        }
    }

    function resetForms() {
        $all('form[data-submitting]').forEach(function (f) { f.removeAttribute('data-submitting'); });
        $all('.btn.is-loading').forEach(function (b) {
            b.classList.remove('is-loading');
            b.removeAttribute('aria-busy');
            b.disabled = false;
        });
        root.classList.remove('is-navigating');
    }

    function initFormLoading() {
        document.addEventListener('submit', function (e) {
            var form = e.target;
            if (!(form instanceof HTMLFormElement)) { return; }
            if (e.defaultPrevented) { return; }              // jQuery Validate đã chặn (form không hợp lệ)
            if (form.hasAttribute('data-no-loading') || form.target === '_blank') { return; }

            if (form.getAttribute('data-submitting') === '1') {
                e.preventDefault();                          // Đang xử lý -> chặn gửi lại
                return;
            }
            form.setAttribute('data-submitting', '1');

            var submitter = e.submitter || form.querySelector('[type="submit"]');
            setLoading(submitter);
            startNavProgress();

            // Vô hiệu hoá các nút submit SAU khi trình duyệt đã đóng gói dữ liệu form
            // (giữ nguyên name/value của nút được bấm).
            setTimeout(function () {
                $all('button[type="submit"], input[type="submit"]', form).forEach(function (b) { b.disabled = true; });
            }, 0);

            // Phòng trường hợp request bị huỷ / tải file: mở khoá sau 20 giây
            setTimeout(function () {
                if (form.getAttribute('data-submitting') === '1') {
                    form.removeAttribute('data-submitting');
                    $all('button[type="submit"], input[type="submit"]', form).forEach(function (b) { b.disabled = false; b.classList.remove('is-loading'); });
                    root.classList.remove('is-navigating');
                }
            }, 20000);
        });

        // Khi người dùng quay lại bằng nút Back (bfcache) -> khôi phục trạng thái
        window.addEventListener('pageshow', function (e) { if (e.persisted) { resetForms(); } });
    }

    /* ------------------------------------------------------------------
       5. THANH TIẾN TRÌNH ĐIỀU HƯỚNG (không trì hoãn điều hướng MVC)
    ------------------------------------------------------------------ */
    function startNavProgress() { root.classList.add('is-navigating'); }

    function initNavProgress() {
        document.addEventListener('click', function (e) {
            if (e.defaultPrevented || e.button !== 0 || e.metaKey || e.ctrlKey || e.shiftKey || e.altKey) { return; }
            var a = e.target.closest('a[href]');
            if (!a) { return; }
            var href = a.getAttribute('href');
            if (!href || href.charAt(0) === '#' || href.indexOf('javascript:') === 0) { return; }
            if (a.target && a.target !== '_self') { return; }
            if (a.hasAttribute('download') || a.hasAttribute('data-bs-toggle')) { return; }
            if (a.origin && a.origin !== window.location.origin) { return; }
            startNavProgress();
        });
    }

    /* ------------------------------------------------------------------
       6. TOOLTIP
    ------------------------------------------------------------------ */
    function initTooltips(ctx) {
        if (!hasBootstrap('Tooltip')) { return; }
        $all('[data-bs-toggle="tooltip"], .btn-icon[title], [data-tooltip][title]', ctx).forEach(function (el) {
            if (window.bootstrap.Tooltip.getInstance(el)) { return; }
            var tip = new window.bootstrap.Tooltip(el, { container: 'body', trigger: 'hover focus' });
            el.addEventListener('click', function () { tip.hide(); });
        });
    }

    /* ------------------------------------------------------------------
       7. COUNT-UP CHO CON SỐ THỐNG KÊ
       <span data-countup="1234" data-decimals="0">1234</span>
       (Giá trị thật đã có sẵn trong HTML -> không JS vẫn hiển thị đúng.)
    ------------------------------------------------------------------ */
    function formatNumber(value, decimals) {
        if (decimals > 0) { return value.toFixed(decimals); }
        return Math.round(value).toLocaleString('vi-VN');
    }

    function runCountUp(el) {
        var target = parseFloat(el.getAttribute('data-countup'));
        if (isNaN(target)) { return; }
        var decimals = parseInt(el.getAttribute('data-decimals') || '0', 10);
        if (reduceMotion || target === 0) { el.textContent = formatNumber(target, decimals); return; }
        var duration = Math.min(1400, 700 + Math.log10(Math.abs(target) + 1) * 220);
        var start = null;
        function step(ts) {
            if (!start) { start = ts; }
            var p = Math.min((ts - start) / duration, 1);
            var eased = 1 - Math.pow(1 - p, 3);
            el.textContent = formatNumber(target * eased, decimals);
            if (p < 1) { window.requestAnimationFrame(step); }
        }
        el.textContent = formatNumber(0, decimals);
        window.requestAnimationFrame(step);
    }

    function initCountUp() {
        var els = $all('[data-countup]');
        if (!els.length) { return; }
        if (!('IntersectionObserver' in window)) { els.forEach(runCountUp); return; }
        var io = new IntersectionObserver(function (entries) {
            entries.forEach(function (entry) {
                if (entry.isIntersecting) { io.unobserve(entry.target); runCountUp(entry.target); }
            });
        }, { threshold: 0.3 });
        els.forEach(function (el) { io.observe(el); });
    }

    /* ------------------------------------------------------------------
       8. LỌC + PHÂN TRANG BẢNG PHÍA CLIENT
       <input data-table-filter="#tbl">     -> lọc tức thời theo nội dung dòng
       <table id="tbl" data-paginate="10"> -> phân trang, hiển thị tổng bản ghi
       Dòng "không có dữ liệu" đánh dấu class .empty-row
    ------------------------------------------------------------------ */
    var pagers = [];

    function dataRows(table) {
        return $all(':scope > tbody > tr', table).filter(function (tr) { return !tr.classList.contains('empty-row') && !tr.classList.contains('filter-empty-row'); });
    }

    function buildPager(table) {
        var size = parseInt(table.getAttribute('data-paginate'), 10) || 10;
        var label = table.getAttribute('data-paginate-label') || 'bản ghi';
        var anchor = table.closest('.table-responsive') || table;
        var footer = document.createElement('div');
        footer.className = 'table-footer';
        footer.innerHTML = '<div class="pager-info"></div><nav aria-label="Phân trang"><ul class="pagination pagination-sm"></ul></nav>';
        anchor.parentNode.insertBefore(footer, anchor.nextSibling);

        var state = { table: table, size: size, page: 1, footer: footer, label: label };

        footer.addEventListener('click', function (e) {
            var link = e.target.closest('[data-page]');
            if (!link) { return; }
            e.preventDefault();
            var p = parseInt(link.getAttribute('data-page'), 10);
            if (!isNaN(p)) { state.page = p; renderPager(state); }
        });

        pagers.push(state);
        renderPager(state);
        return state;
    }

    function pageItem(label, page, opts) {
        opts = opts || {};
        var cls = 'page-item' + (opts.active ? ' active' : '') + (opts.disabled ? ' disabled' : '');
        if (opts.ellipsis) { return '<li class="page-item disabled"><span class="page-link">…</span></li>'; }
        return '<li class="' + cls + '"><a class="page-link" href="#" data-page="' + page + '"' +
            (opts.aria ? ' aria-label="' + opts.aria + '"' : '') + (opts.active ? ' aria-current="page"' : '') + '>' + label + '</a></li>';
    }

    function renderPager(state) {
        var rows = dataRows(state.table);
        var visible = rows.filter(function (r) { return !r.classList.contains('is-filtered-out'); });
        var total = visible.length;
        var pages = Math.max(1, Math.ceil(total / state.size));
        if (state.page > pages) { state.page = pages; }
        var from = (state.page - 1) * state.size;
        var to = Math.min(from + state.size, total);

        rows.forEach(function (r) { r.classList.remove('is-paged-out'); });
        visible.forEach(function (r, i) { if (i < from || i >= to) { r.classList.add('is-paged-out'); } });

        var info = state.footer.querySelector('.pager-info');
        info.innerHTML = total === 0
            ? 'Không có ' + state.label + ' phù hợp'
            : 'Hiển thị <strong>' + (from + 1) + '–' + to + '</strong> trên tổng <strong>' + total.toLocaleString('vi-VN') + '</strong> ' + state.label;

        var ul = state.footer.querySelector('.pagination');
        if (pages <= 1) { ul.innerHTML = ''; return; }
        var html = pageItem('<i class="fa-solid fa-chevron-left"></i>', state.page - 1, { disabled: state.page === 1, aria: 'Trang trước' });
        var last = 0;
        for (var p = 1; p <= pages; p++) {
            if (p === 1 || p === pages || Math.abs(p - state.page) <= 1) {
                if (last && p - last > 1) { html += pageItem('', 0, { ellipsis: true }); }
                html += pageItem(String(p), p, { active: p === state.page });
                last = p;
            }
        }
        html += pageItem('<i class="fa-solid fa-chevron-right"></i>', state.page + 1, { disabled: state.page === pages, aria: 'Trang sau' });
        ul.innerHTML = html;
    }

    function refreshTable(table) {
        pagers.forEach(function (s) { if (s.table === table) { s.page = 1; renderPager(s); } });
    }

    function initTableFilter() {
        $all('[data-table-filter]').forEach(function (input) {
            var table = $(input.getAttribute('data-table-filter'));
            if (!table) { return; }
            var colCount = (table.tHead && table.tHead.rows[0]) ? table.tHead.rows[0].cells.length : 1;
            var emptyRow = document.createElement('tr');
            emptyRow.className = 'filter-empty-row d-none';
            emptyRow.innerHTML = '<td colspan="' + colCount + '"><div class="empty-state py-4"><div class="empty-icon"><i class="fa-solid fa-magnifying-glass"></i></div><div class="empty-title">Không tìm thấy kết quả phù hợp</div><p>Thử từ khoá khác hoặc xoá bộ lọc.</p></div></td>';
            if (table.tBodies[0]) { table.tBodies[0].appendChild(emptyRow); }

            function apply() {
                var keyword = input.value.toLowerCase().trim();
                var rows = dataRows(table);
                var matched = 0;
                rows.forEach(function (row) {
                    var text = '';
                    for (var i = 0; i < row.cells.length - 1; i++) { text += ' ' + row.cells[i].textContent; }
                    var ok = !keyword || text.toLowerCase().indexOf(keyword) !== -1;
                    row.classList.toggle('is-filtered-out', !ok);
                    if (ok) { matched++; }
                });
                emptyRow.classList.toggle('d-none', !(rows.length && matched === 0));
                refreshTable(table);
            }
            input.addEventListener('input', apply);
            if (input.value) { apply(); }
        });
    }

    function initPagers() { $all('table[data-paginate]').forEach(buildPager); }

    /* ------------------------------------------------------------------
       9. TIỆN ÍCH NHỎ
    ------------------------------------------------------------------ */
    function initPasswordToggles() {
        document.addEventListener('click', function (e) {
            var btn = e.target.closest('[data-toggle-password]');
            if (!btn) { return; }
            var input = $(btn.getAttribute('data-toggle-password'));
            if (!input) { return; }
            var show = input.type === 'password';
            input.type = show ? 'text' : 'password';
            btn.setAttribute('aria-pressed', String(show));
            btn.setAttribute('aria-label', show ? 'Ẩn mật khẩu' : 'Hiện mật khẩu');
            var icon = btn.querySelector('i');
            if (icon) {
                icon.classList.toggle('fa-eye', !show); icon.classList.toggle('fa-eye-slash', show);
                icon.classList.toggle('bi-eye', !show); icon.classList.toggle('bi-eye-slash', show);
            }
            input.focus();
        });
    }

    function initToday() {
        $all('[data-today]').forEach(function (el) {
            try {
                var s = new Date().toLocaleDateString('vi-VN', { weekday: 'long', day: '2-digit', month: '2-digit', year: 'numeric' });
                el.textContent = s.charAt(0).toUpperCase() + s.slice(1);
            } catch (e) { /* giữ giá trị server render */ }
        });
    }

    function initAutofocusEnd() {
        var el = $('[data-autofocus-end]');
        if (!el || !el.value) { return; }
        el.focus({ preventScroll: true });
        try { var len = el.value.length; el.setSelectionRange(len, len); } catch (e) { /* type không hỗ trợ */ }
    }

    // Modal nằm trong phần tử có animation transform -> chuyển ra body để định vị chuẩn
    function hoistModals() {
        $all('.app-content .modal').forEach(function (m) { document.body.appendChild(m); });
    }

    // Việt hoá thông báo validation mặc định (jQuery Validate + message tiếng Anh mặc định của MVC)
    function localizeValidation() {
        $all('[data-val-required]').forEach(function (input) {
            var msg = input.getAttribute('data-val-required') || '';
            if (/^The .+ field is required\.$/.test(msg)) {
                var label = input.id ? $('label[for="' + input.id + '"]') : null;
                var name = label ? label.textContent.replace('*', '').trim() : '';
                input.setAttribute('data-val-required', name ? 'Vui lòng nhập ' + name.toLowerCase() + '.' : 'Trường này là bắt buộc.');
            }
        });
        $all('[data-val-number]').forEach(function (input) {
            if (/^The field .+ must be a number\.$/.test(input.getAttribute('data-val-number') || '')) {
                input.setAttribute('data-val-number', 'Vui lòng nhập một số hợp lệ.');
            }
        });
        document.addEventListener('DOMContentLoaded', function () {
            var jq = window.jQuery;
            if (jq && jq.validator && jq.validator.messages) {
                jq.extend(jq.validator.messages, {
                    required: 'Trường này là bắt buộc.',
                    number: 'Vui lòng nhập một số hợp lệ.',
                    digits: 'Vui lòng chỉ nhập chữ số.',
                    email: 'Email không hợp lệ.',
                    max: jq.validator.format('Giá trị không được lớn hơn {0}.'),
                    min: jq.validator.format('Giá trị không được nhỏ hơn {0}.'),
                    maxlength: jq.validator.format('Không được vượt quá {0} ký tự.'),
                    minlength: jq.validator.format('Cần ít nhất {0} ký tự.'),
                    range: jq.validator.format('Giá trị phải trong khoảng {0} đến {1}.'),
                    step: jq.validator.format('Giá trị phải là bội số của {0}.')
                });
            }
        });
    }

    /* Biểu đồ: theme đồng bộ cho Chart.js (gọi từ trang sau khi đã tải Chart.js) */
    function applyChartTheme(Chart) {
        if (!Chart || !Chart.defaults) { return; }
        Chart.defaults.font.family = "'Inter', system-ui, sans-serif";
        Chart.defaults.font.size = 12;
        Chart.defaults.color = '#64748B';
        Chart.defaults.borderColor = '#EEF2F7';
        Chart.defaults.plugins.tooltip.backgroundColor = '#0F172A';
        Chart.defaults.plugins.tooltip.padding = 10;
        Chart.defaults.plugins.tooltip.cornerRadius = 8;
        Chart.defaults.plugins.tooltip.titleFont = { weight: '600' };
        Chart.defaults.plugins.legend.labels.usePointStyle = true;
        Chart.defaults.plugins.legend.labels.boxWidth = 8;
        if (reduceMotion) { Chart.defaults.animation = false; }
        else { Chart.defaults.animation.duration = 900; Chart.defaults.animation.easing = 'easeOutQuart'; }
    }

    /* ------------------------------------------------------------------
       KHỞI TẠO
    ------------------------------------------------------------------ */
    function init() {
        hoistModals();
        initSidebar();
        initFlash();
        initNetworkFeedback();
        initConfirmTriggers();
        initFormLoading();
        initNavProgress();
        initTooltips();
        initCountUp();
        initTableFilter();
        initPagers();
        initPasswordToggles();
        initToday();
        initAutofocusEnd();
    }

    window.UMS = {
        toast: toast,
        confirm: confirmDialog,
        refreshTable: refreshTable,
        initTooltips: initTooltips,
        applyChartTheme: applyChartTheme,
        reduceMotion: reduceMotion
    };

    // Chạy ngay: phải đổi thông báo trước khi jquery.validate.unobtrusive phân tích form
    localizeValidation();

    if (document.readyState === 'loading') { document.addEventListener('DOMContentLoaded', init); }
    else { init(); }
})(window, document);
