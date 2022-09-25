; ---------------------------------------------------------------------
; Constants
; ---------------------------------------------------------------------

; Note that some of the constants are declared as variables.
; This way their addresses (i.e. names) can be passed to instructions.
; CIN and CSTR define const of integer or string type
; DC, DD and DQ define variables with size of 1, 2 and 4 characters.

DQ DESIRED_PRIMES_COUNT = 100
CINT SIEVE_BUFFER_SIZE = 550
CINT STA_ADDRESS = 212

DC FAILURE = 'FAIL'
DC SUCCESS = 'SUCCESS '
DC SIEVE_BUFFER_TOO_SMALL = 'SIEVE BUFFER TOO SMALL'

DQ ONE = 1 ; Used for incrementation

; ---------------------------------------------------------------------
; Actual variables (that change their values)
; ---------------------------------------------------------------------

DQ Temp = 0
DQ PrimesFound = 0

DQ Prime = 0
DQ Pointer = 0

; These variables must be last - they define the range of the sieve buffer.
; They contain the addresses of the first and last cell.
DQ SieveBegin = SieveEnd + 4      ; Sieve starts right after SieveEnd
DQ SieveEnd = [SieveBegin] + SIEVE_BUFFER_SIZE

; ---------------------------------------------------------------------
; Code
; ---------------------------------------------------------------------

; Fill the sieve buffer with zeroes (value of '1' means 'marked').
; Note the indirect addressing - the variables contain addresses.
SF 0, {SieveBegin}, {SieveEnd}

; Mark the first two sieve cells so the first prime number will be 2
DL 4, SieveBegin, Pointer
SF 1, {Pointer}, {Pointer}
ADD 4, Pointer + 3, ONE + 3       ; Funny way to increment, isn't it
SF 1, {Pointer}, {Pointer}

primes:

LSL 1, {SieveBegin}, {SieveEnd}   ; Search for the first non-marked cell
REG 2, 0, 0                       ; Store search result in STA
CTC 1, found, error               ; PRI is not changed by REG
REG 1, Temp + 3, buffer_too_small ; All sieve cells are already marked

found:
DL 4, STA_ADDRESS, Prime          ; Put in Prime the non-marked cell
ADD 4, Prime + 3, ONE + 3         ; This is due to LSL output specifics
SUB 4, Prime + 3, SieveBegin + 3  ; Calculate the prime number

; ---------------------------------------------------------------------
; Prepare for the loop that marks all multiples of Prime.
; Pointer will have values from SieveBegin to SieveEnd and through
; indirect addressing corresponding sieve cells will be set to '1'.
DL 4, SieveBegin, Pointer

mark:
SF 1, {Pointer}, {Pointer}
ADD 4, Pointer + 3, Prime + 3
COM 4, Pointer, SieveEnd
CTC 1, next_prime, mark
REG 1, Temp + 3, mark
; Finished marking all sieve cells that are dividable by Prime
; ---------------------------------------------------------------------

next_prime:
TWN 0, Prime, Prime + 3           ; Print the current prime number
ADD 4, PrimesFound + 3, ONE + 3
COM 4, PrimesFound, DESIRED_PRIMES_COUNT
CTC 1, end, primes                ; Loop until DESIRED_PRIMES_COUNT primes are found

; Hooray, task solved successfully
end:
TWN 0, SUCCESS, SUCCESS + 7
TWN 0, PrimesFound, PrimesFound + 3
HLT 0, 0, 0

; Keep increasing the size of the sieve buffer until successful exit
buffer_too_small:
TWN 0, SIEVE_BUFFER_TOO_SMALL, SIEVE_BUFFER_TOO_SMALL + 21
TWN 0, PrimesFound, PrimesFound + 3
HLT 0, 0, 0

; Internal error - should never go here
error:
TWN 0, FAILURE, FAILURE + 3
TWN 0, PrimesFound, PrimesFound + 3
HLT 0, 0, 0
