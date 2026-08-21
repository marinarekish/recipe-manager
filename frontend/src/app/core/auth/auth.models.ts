// JSON parse

/** One role from the API (users.roles[]) */
export interface RoleDto {
  roleId: number;
  name: string;
}

/** User profile from the API */
export interface UserDto {
  userId: number;
  firstName: string;
  lastName: string;
  email: string;
  phone: string | null;
  roles: RoleDto[];
  createdAt: string;
}

/** Body of successful POST /api/auth/verify-code */
export interface AuthResponse {
  user: UserDto;
  accessToken: string;
  expiresIn: number;
}
