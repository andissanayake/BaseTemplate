/* eslint-disable @typescript-eslint/no-explicit-any */
import { useEffect } from "react";

export function useAsyncEffect(effect: () => Promise<void>, deps: any[]) {
  useEffect(() => {
    void effect();
  }, deps);
}
