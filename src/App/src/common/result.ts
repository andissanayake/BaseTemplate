export interface Result<T = unknown> {
  code: string;
  message?: string;
  details?: Record<string, string[]>;
  data?: T;
}
