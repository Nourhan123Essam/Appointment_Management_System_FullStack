export interface Result<T> {
  succeeded: boolean;
  message: string;
  data?: T;
}