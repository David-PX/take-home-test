export interface AuthRequestDto {
  email: string;
  passwordHash: string;
}

export interface AuthResponseDto {
  accessToken: string;
}
