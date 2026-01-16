export type GlobalNotice = {
  kind: 'rate-limit' | 'network' | 'auth-lock' | 'info' | 'warning' | 'success' | 'error'
  message: string
  retryAfterSeconds?: number
};

const listeners = new Set<(notice: GlobalNotice) => void>();

export const subscribeToNotices = (listener: (notice: GlobalNotice) => void) => {
  listeners.add(listener);
  return () => listeners.delete(listener);
};

export const publishNotice = (notice: GlobalNotice) => {
  listeners.forEach((listener) => listener(notice));
};
