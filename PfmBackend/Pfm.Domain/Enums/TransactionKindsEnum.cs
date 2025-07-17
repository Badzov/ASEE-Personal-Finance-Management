using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pfm.Domain.Enums
{
    public enum TransactionKindsEnum
    {
        dep,        // Deposit  
        wdw,        // Withdrawal  
        pmt,        // Payment  
        fee,        // Fee  
        inc,        // Interest  
        rev,        // Reversal  
        adj,        // Adjustment  
        lnd,        // LoanDisbursement  
        lnr,        // LoanRepayment  
        fcx,        // ForeignExchange  
        aop,        // AccountOpening  
        acl,        // AccountClosing  
        spl,        // SplitPayment  
        sal         // Salary  
    }

}
