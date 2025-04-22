/* eslint-disable @typescript-eslint/no-explicit-any */
// utils/useAsyncEffect.ts
import { useEffect } from "react";

export function useAsyncEffect(effect: () => Promise<void>, deps: any[]) {
  useEffect(() => {
    void effect();
  }, deps);
}
