export interface User {
  id: string
  emailAddress: string
  roles?: string
  firstname?: string
  lastname?: string
  lastFailedValidationTimestamp?: string
  lastSuccessfulValidationTimestamp?: string
  mfaActive: boolean
}
