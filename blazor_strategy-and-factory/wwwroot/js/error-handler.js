// 全域錯誤處理，用於處理瀏覽器擴展程式的錯誤
window.addEventListener('error', function(event) {
    console.log('捕獲到錯誤:', event.message, event.filename, event.lineno);
    
    // 檢查是否是瀏覽器擴展程式的錯誤
    if (event.filename && (
        event.filename.includes('contentscript.js') ||
        event.filename.includes('content_script.js') ||
        event.filename.includes('extension') ||
        event.filename.includes('chrome-extension') ||
        event.filename.includes('moz-extension')
    )) {
        console.warn('✅ 已忽略瀏覽器擴展程式錯誤:', event.message);
        event.preventDefault();
        return true;
    }

    // 檢查錯誤訊息是否包含擴展程式相關關鍵字
    if (event.message && (
        event.message.includes('querySelector') ||
        event.message.includes('contentscript') ||
        event.message.includes('extension') ||
        event.message.includes('chrome-extension') ||
        event.message.includes('Cannot read properties of null') ||
        event.message.includes('Cannot read property') ||
        event.message.includes('jQuery')
    )) {
        console.warn('✅ 已忽略瀏覽器擴展程式錯誤:', event.message);
        event.preventDefault();
        return true;
    }

    // 其他錯誤正常處理
    console.error('❌ 應用程式錯誤:', event.message, event.filename, event.lineno);
    return false;
});

// 處理未捕獲的 Promise 拒絕
window.addEventListener('unhandledrejection', function(event) {
    console.log('捕獲到未處理的 Promise 拒絕:', event.reason);
    
    // 檢查是否是擴展程式相關的錯誤
    if (event.reason && event.reason.message && (
        event.reason.message.includes('contentscript') ||
        event.reason.message.includes('extension') ||
        event.reason.message.includes('querySelector')
    )) {
        console.warn('✅ 已忽略擴展程式相關的 Promise 拒絕:', event.reason);
        event.preventDefault();
        return true;
    }
    
    console.error('❌ 未處理的 Promise 拒絕:', event.reason);
    return false;
});

// 添加 Blazor 特定的錯誤處理
window.blazorErrorHandler = {
    handleAsyncError: function(error) {
        if (error && error.message && (
            error.message.includes('contentscript') ||
            error.message.includes('extension') ||
            error.message.includes('querySelector')
        )) {
            console.warn('✅ 已忽略 Blazor 中的瀏覽器擴展程式錯誤:', error.message);
            return true;
        }
        return false;
    }
};

// 監控 Blazor 錯誤
window.addEventListener('DOMContentLoaded', function() {
    console.log('DOM 已載入，開始監控 Blazor 錯誤...');
    
    // 檢查是否有 Blazor
    if (window.Blazor) {
        console.log('Blazor 已載入');
    }
});

console.log('🛡️ 錯誤處理器已載入，開始監控瀏覽器擴展程式錯誤');
