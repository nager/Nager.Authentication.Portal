import { MfaError } from './MfaError'
import { MfaSuccess } from './MfaSuccess'

export type MfaResponse = MfaSuccess | MfaError

export function instanceOfMfaError (mfaResponse: MfaError | MfaSuccess) {
  return 'error' in mfaResponse
}
