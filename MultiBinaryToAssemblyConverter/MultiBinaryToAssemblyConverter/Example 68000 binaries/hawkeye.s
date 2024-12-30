;APS00000000000000000000000000000000000000000000000000000000000000000000000000000000
                *=$0000
                MOVEA.L $0004.W,A6
                LEA $007E,A1
                JSR -408(A6)
                MOVE.L D0,$00024DEE
;                BSR $009a
                LEA $00DFF000,A5
                LEA $00024F72,A0
                MOVE.L A0,$0080(A5)
                MOVE.W #$80C0,$0096(A5)
                MOVE.W #$8010,$009A(A5)
;                BSR.W $04a2
;                BSR.W $058e
branch0         CMP #$00FF,$0006(A5)
                BNE.W branch0
;                BSR.W $0aa4
;                BSR.W $015c
;                BSR.W $05e6
                MOVE.B $00BFEC01,D0
                EOR.B #$00FF,D0
;                ROR #$,D0
                CMP.B #$0045,D0
;                BEQ $006C
                BTST.B #$0006,$00BFE001
                BNE branch0
;                BSR.W $058e
                MOVE.L $0024(PC),A1
                MOVE.L $0026(A1),$0080
                JSR -414(A6)
;                MOVEQ #$,D0
                RTS 
;                BEQ $00F8
;                BSR.W $00F8
;                BVC.W $
;                BLS.W $
;                MOVEA.L A46962,A7
;                MOVEQ #$,D1
;                MOVEQ #$,D1
                OR #$0000,D0
                OR #$43F9,D0
                OR #$D522,D3
                LEA $00024F94,A3
                LEA $00024F98,A2
                MOVE.L A1,D1
                MOVE.W D1,(A2)
                MOVE.W (A2),D2
                SWAP.W D1
                MOVE.W D1,(A3)
                MOVE.W (A3),D3
                LEA $0003FD22,A1
                LEA $00024F9C,A3
                LEA $00024FA0,A2
                MOVE.L A1,D1
                MOVE.W D1,(A2)
                MOVE.W (A2),D2
                SWAP.W D1
                MOVE.W D1,(A3)
                MOVE.W (A3),D3
                LEA $00042522,A1
                LEA $00024FA4,A3
                LEA $00024FA8,A2
                MOVE.L A1,D1
                MOVE.W D1,(A2)
                MOVE.W (A2),D2
                SWAP.W D1
                MOVE.W D1,(A3)
                MOVE.W (A3),D3
                LEA $00044D22,A1
                LEA $00024FAC,A3
                LEA $00024FB0,A2
                MOVE.L A1,D1
                MOVE.W D1,(A2)
                MOVE.W (A2),D2
                SWAP.W D1
                MOVE.W D1,(A3)
                MOVE.W (A3),D3
                LEA $00047522,A1
                LEA $00024FB4,A3
                LEA $00024FB8,A2
                MOVE.L A1,D1
                MOVE.W D1,(A2)
                MOVE.W (A2),D2
                SWAP.W D1
                MOVE.W D1,(A3)
                MOVE.W (A3),D3
                LEA $00049D22,A1
                LEA $00025088,A3
                LEA $0002508C,A2
                MOVE.L A1,D1
                MOVE.W D1,(A2)
                MOVE.W (A2),D2
                SWAP.W D1
                MOVE.W D1,(A3)
                MOVE.W (A3),D3
                RTS 
                OR #$4EF6,D2
                OR #$0000,D0
                OR #$0000,D0
                CLR.L D0
                MOVE.L $00024EAC,D0
                MOVE.L $00024EA8,A0
;                MOVE.L A008000002,D05070
;                MOVE.L A008000002,D051EC
                MOVE.L $00024EAC,D0
                CMP.W #$003C,D0
;                BEQ 0192
;                ADDQ.L #$,D0
                MOVE.L D0,$00024EAC
                RTS 
;                MOVE.L #$,$
                RTS 
                ILLEGAL 
                ILLEGAL 
;                CAS.L D60EEE0DDD,D3
                BSET.B D6,(A5)+
                ILLEGAL 
                ILLEGAL 
                ILLEGAL 
                ILLEGAL 
;                EOR.L #$0AAA0999,A20999
                ILLEGAL 
                ILLEGAL 
;                BCHG D3,A2
                SUB.W #$0444,D4
;                BTST.B D1,A3
                OR #$0000,D0
                BTST.B D0,(A1)
                BTST.B D0,(A1)
;                AND.B #$0222,-(A2)
;                BTST.B D1,A3
                ADD.W #$0666,-(A6)
;                BCHG D3,A2
;                EOR.L #$0AAA0BBB,A20BBB
                ILLEGAL 
                ILLEGAL 
                BSET.B D6,(A5)+
                BSET.B D6,(A5)+
;                CAS.L D60EEE0FFF,D3
                ILLEGAL 
;                BSET.B D0,D2
                OR #$01FC,D0
                OR #$0100,D0
;                ADDQ.B #$,D0
;                BTST.B D0,D6
                OR #$0092,D0
;                OR #$009400CC,A4
;                MOVEP.W A00000,D0
;                MOVEP.W A20000,D0
                ILLEGAL 
                OR #$00E2,D0
                OR #$00E4,D0
                OR #$00E6,D0
                OR #$00E8,D0
                OR #$00EA,D0
                OR #$00EC,D0
                OR #$00EE,D0
                OR #$00F0,D0
                OR #$00F2,D0
