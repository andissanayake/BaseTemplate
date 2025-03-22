import { AppUser } from "./authSlice";
import { Policies } from "./PoliciesEnum";

export const authPolicy = (policy: Policies, user: AppUser | null): boolean => {
  switch (policy) {
    case Policies.User: {
      return user ? true : false;
    }
    case Policies.Guest: {
      return user ? false : true;
    }
    default:
      return false;
  }
};
